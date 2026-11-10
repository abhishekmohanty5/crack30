import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { McqQuizItem, McqAttemptResponse } from '../types/mcq';
import {
  ArrowLeft,
  Clock,
  CheckCircle2,
  AlertCircle,
  HelpCircle,
  Award,
  RotateCcw
} from 'lucide-react';

export const McqQuizPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [questions, setQuestions] = useState<McqQuizItem[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [userAnswers, setUserAnswers] = useState<Record<number, string>>({});
  const [loading, setLoading] = useState(true);

  // Timer & Quiz State
  const [timeLeft, setTimeLeft] = useState(600); // 10 minutes for 10 questions
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [results, setResults] = useState<Record<number, McqAttemptResponse>>({});
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const fetchQuiz = async () => {
      try {
        const res = await api.get<McqQuizItem[]>(`/mcqs/chapter/${id}/quiz?count=10`);
        setQuestions(res.data);
      } catch (err) {
        console.error('Failed to load quiz', err);
      } finally {
        setLoading(false);
      }
    };

    fetchQuiz();
  }, [id]);

  useEffect(() => {
    if (isSubmitted || loading || questions.length === 0) return;

    const timer = setInterval(() => {
      setTimeLeft((prev) => {
        if (prev <= 1) {
          clearInterval(timer);
          handleSubmitQuiz();
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [isSubmitted, loading, questions.length]);

  const handleSelectOption = (opt: string) => {
    if (isSubmitted) return;
    setUserAnswers((prev) => ({ ...prev, [currentIndex]: opt }));
  };

  const handleSubmitQuiz = async () => {
    if (submitting || isSubmitted) return;
    setSubmitting(true);

    const resultMap: Record<number, McqAttemptResponse> = {};
    for (let i = 0; i < questions.length; i++) {
      const q = questions[i];
      const chosen = userAnswers[i] || 'A';
      try {
        const res = await api.post<McqAttemptResponse>(`/mcqs/${q.id}/attempt`, {
          chosenOption: chosen,
          timeTakenSec: 15,
        });
        resultMap[i] = res.data;
      } catch (err) {
        console.error('Failed to score question', q.id, err);
      }
    }

    setResults(resultMap);
    setIsSubmitted(true);
    setSubmitting(false);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600" />
      </div>
    );
  }

  if (questions.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-slate-500">Not enough MCQs to generate a quiz for this chapter.</p>
        <Link to={`/chapters/${id}`} className="mt-3 text-indigo-600 font-semibold inline-block">
          ← Back to Chapter
        </Link>
      </div>
    );
  }

  const formatTime = (secs: number) => {
    const m = Math.floor(secs / 60);
    const s = secs % 60;
    return `${m}:${s < 10 ? '0' : ''}${s}`;
  };

  const currentQ = questions[currentIndex];
  const answeredCount = Object.keys(userAnswers).length;
  const score = Object.values(results).filter((r) => r.isCorrect).length;

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      {/* Top Bar with Timer and Summary */}
      <div className="flex items-center justify-between bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-4 rounded-2xl shadow-sm">
        <Link
          to={`/chapters/${id}`}
          className="inline-flex items-center gap-1.5 text-xs font-semibold text-slate-500 hover:text-indigo-600 transition"
        >
          <ArrowLeft className="w-4 h-4" /> Exit
        </Link>

        {!isSubmitted ? (
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-1.5 px-3 py-1 rounded-xl bg-amber-50 dark:bg-amber-950/40 border border-amber-200 dark:border-amber-900 text-amber-700 dark:text-amber-400 text-xs font-bold">
              <Clock className="w-3.5 h-3.5" />
              <span>{formatTime(timeLeft)}</span>
            </div>
            <span className="text-xs text-slate-500 font-medium">
              Answered: {answeredCount} / {questions.length}
            </span>
          </div>
        ) : (
          <div className="flex items-center gap-2">
            <span className="px-3 py-1 rounded-xl bg-indigo-50 dark:bg-indigo-950/60 text-indigo-600 dark:text-indigo-400 text-xs font-bold">
              Score: {score} / {questions.length} ({Math.round((score / questions.length) * 100)}%)
            </span>
          </div>
        )}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        {/* Question Palette Sidebar */}
        <div className="md:col-span-1 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-4 rounded-2xl space-y-3 h-fit">
          <h3 className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Questions
          </h3>
          <div className="grid grid-cols-5 gap-2">
            {questions.map((_, idx) => {
              const isAnswered = userAnswers[idx] !== undefined;
              const isCurrent = currentIndex === idx;
              let bg = 'bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-400';

              if (isSubmitted) {
                const res = results[idx];
                bg = res?.isCorrect
                  ? 'bg-emerald-600 text-white'
                  : 'bg-rose-600 text-white';
              } else if (isAnswered) {
                bg = 'bg-indigo-600 text-white';
              }

              return (
                <button
                  key={idx}
                  onClick={() => setCurrentIndex(idx)}
                  className={`h-9 w-9 rounded-xl text-xs font-bold transition flex items-center justify-center ${bg} ${
                    isCurrent ? 'ring-2 ring-indigo-400 ring-offset-2 dark:ring-offset-slate-900' : ''
                  }`}
                >
                  {idx + 1}
                </button>
              );
            })}
          </div>

          {!isSubmitted && (
            <button
              onClick={handleSubmitQuiz}
              disabled={submitting}
              className="w-full mt-4 py-2 px-3 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-bold shadow-sm transition disabled:opacity-50"
            >
              {submitting ? 'Scoring...' : 'Submit Quiz'}
            </button>
          )}
        </div>

        {/* Question Panel */}
        <div className="md:col-span-3 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 sm:p-8 rounded-2xl space-y-6 shadow-sm">
          <div className="space-y-1">
            <span className="text-xs font-bold text-indigo-600 dark:text-indigo-400">
              Question {currentIndex + 1} of {questions.length}
            </span>
            <h2 className="text-base sm:text-lg font-bold text-slate-900 dark:text-white">
              {currentQ.question}
            </h2>
          </div>

          {/* Options */}
          <div className="space-y-3">
            {[
              { key: 'A', text: currentQ.optionA },
              { key: 'B', text: currentQ.optionB },
              { key: 'C', text: currentQ.optionC },
              { key: 'D', text: currentQ.optionD },
            ].map((opt) => {
              const isSelected = userAnswers[currentIndex] === opt.key;
              let style =
                'border-slate-200 dark:border-slate-800 hover:border-indigo-400 hover:bg-slate-50 dark:hover:bg-slate-800/60';

              if (isSubmitted) {
                const res = results[currentIndex];
                if (opt.key === res?.correctOption) {
                  style = 'border-emerald-500 bg-emerald-50 dark:bg-emerald-950/40 font-semibold';
                } else if (isSelected && !res?.isCorrect) {
                  style = 'border-rose-500 bg-rose-50 dark:bg-rose-950/40';
                } else {
                  style = 'opacity-40 border-slate-200 dark:border-slate-800';
                }
              } else if (isSelected) {
                style = 'border-indigo-600 bg-indigo-50/80 dark:bg-indigo-950/50 font-semibold';
              }

              return (
                <button
                  key={opt.key}
                  disabled={isSubmitted}
                  onClick={() => handleSelectOption(opt.key)}
                  className={`w-full flex items-center gap-3 p-4 rounded-xl border text-left text-sm transition ${style}`}
                >
                  <span className="w-6 h-6 rounded-lg bg-slate-100 dark:bg-slate-800 flex items-center justify-center font-bold text-xs text-slate-700 dark:text-slate-300">
                    {opt.key}
                  </span>
                  <span className="text-slate-800 dark:text-slate-200">{opt.text}</span>
                </button>
              );
            })}
          </div>

          {/* Results Feedback per question when submitted */}
          {isSubmitted && results[currentIndex] && (
            <div className="p-4 rounded-xl bg-slate-50 dark:bg-slate-800/70 border border-slate-200 dark:border-slate-700 space-y-1 text-xs">
              <div className="font-bold text-slate-800 dark:text-slate-200">
                {results[currentIndex].isCorrect ? '✅ Correct' : `❌ Incorrect (Answer: ${results[currentIndex].correctOption})`}
              </div>
              <p className="text-slate-600 dark:text-slate-300">
                {results[currentIndex].explanation}
              </p>
            </div>
          )}

          {/* Prev / Next controls */}
          <div className="flex items-center justify-between pt-4 border-t border-slate-100 dark:border-slate-800">
            <button
              onClick={() => setCurrentIndex((prev) => Math.max(0, prev - 1))}
              disabled={currentIndex === 0}
              className="px-4 py-2 rounded-xl text-xs font-semibold text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800 disabled:opacity-40 transition"
            >
              Previous
            </button>
            <button
              onClick={() => setCurrentIndex((prev) => Math.min(questions.length - 1, prev + 1))}
              disabled={currentIndex === questions.length - 1}
              className="px-4 py-2 rounded-xl text-xs font-semibold text-white bg-indigo-600 hover:bg-indigo-700 disabled:opacity-40 transition"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
