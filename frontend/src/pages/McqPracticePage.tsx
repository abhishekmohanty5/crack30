import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../services/api';
import { Mcq, McqAttemptResponse } from '../types/mcq';
import {
  ArrowLeft,
  CheckCircle2,
  XCircle,
  HelpCircle,
  ArrowRight,
  RotateCcw,
  Sparkles
} from 'lucide-react';

export const McqPracticePage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [questions, setQuestions] = useState<Mcq[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [loading, setLoading] = useState(true);

  // Attempt states
  const [selectedOption, setSelectedOption] = useState<string | null>(null);
  const [attemptResult, setAttemptResult] = useState<McqAttemptResponse | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [startTime, setStartTime] = useState<number>(Date.now());
  const [correctCount, setCorrectCount] = useState(0);

  useEffect(() => {
    const fetchQuestions = async () => {
      try {
        const res = await api.get<{ items: Mcq[] }>(`/mcqs/chapter/${id}?page=1&pageSize=50`);
        setQuestions(res.data.items || []);
      } catch (err) {
        console.error('Failed to load MCQs', err);
      } finally {
        setLoading(false);
      }
    };

    fetchQuestions();
  }, [id]);

  useEffect(() => {
    setStartTime(Date.now());
    setSelectedOption(null);
    setAttemptResult(null);
  }, [currentIndex]);

  const handleSelectOption = async (opt: string) => {
    if (selectedOption !== null || submitting) return; // already answered
    setSelectedOption(opt);
    setSubmitting(true);

    const timeTakenSec = Math.max(1, Math.round((Date.now() - startTime) / 1000));
    const currentQ = questions[currentIndex];

    try {
      const res = await api.post<McqAttemptResponse>(`/mcqs/${currentQ.id}/attempt`, {
        chosenOption: opt,
        timeTakenSec,
      });
      setAttemptResult(res.data);
      if (res.data.isCorrect) {
        setCorrectCount((prev) => prev + 1);
      }
    } catch (err) {
      console.error('Attempt submission failed', err);
    } finally {
      setSubmitting(false);
    }
  };

  const nextQuestion = () => {
    if (currentIndex < questions.length - 1) {
      setCurrentIndex((prev) => prev + 1);
    }
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
        <p className="text-slate-500">No MCQs found for this chapter.</p>
        <Link to={`/chapters/${id}`} className="mt-3 text-indigo-600 font-semibold inline-block">
          ← Back to Chapter
        </Link>
      </div>
    );
  }

  const currentQ = questions[currentIndex];
  const isFinished = currentIndex >= questions.length;
  const options = [
    { key: 'A', text: currentQ.optionA },
    { key: 'B', text: currentQ.optionB },
    { key: 'C', text: currentQ.optionC },
    { key: 'D', text: currentQ.optionD },
  ];

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      {/* Header bar */}
      <div className="flex items-center justify-between">
        <Link
          to={`/chapters/${id}`}
          className="inline-flex items-center gap-2 text-xs font-semibold text-slate-500 hover:text-indigo-600 transition"
        >
          <ArrowLeft className="w-3.5 h-3.5" />
          Back to Chapter
        </Link>

        <div className="flex items-center gap-3">
          <span className="text-xs font-bold text-slate-400">
            Question {currentIndex + 1} of {questions.length}
          </span>
          <span className="px-2.5 py-1 rounded-full text-xs font-bold bg-emerald-50 text-emerald-600 dark:bg-emerald-950/60 dark:text-emerald-400">
            Score: {correctCount} / {currentIndex + (selectedOption ? 1 : 0)}
          </span>
        </div>
      </div>

      {/* Progress Bar */}
      <div className="h-1.5 w-full bg-slate-200 dark:bg-slate-800 rounded-full overflow-hidden">
        <div
          className="h-full bg-indigo-600 transition-all duration-300"
          style={{ width: `${((currentIndex + 1) / questions.length) * 100}%` }}
        />
      </div>

      {/* Question Card */}
      <div className="rounded-3xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 p-6 sm:p-8 shadow-sm space-y-6">
        <div className="flex items-start justify-between gap-4">
          <h2 className="text-lg sm:text-xl font-bold text-slate-900 dark:text-white leading-relaxed">
            {currentQ.question}
          </h2>
          <span
            className={`text-[10px] font-bold px-2 py-0.5 rounded-full shrink-0 ${
              currentQ.difficulty === 'Easy'
                ? 'bg-emerald-50 text-emerald-600 dark:bg-emerald-950 dark:text-emerald-400'
                : currentQ.difficulty === 'Medium'
                ? 'bg-amber-50 text-amber-600 dark:bg-amber-950 dark:text-amber-400'
                : 'bg-rose-50 text-rose-600 dark:bg-rose-950 dark:text-rose-400'
            }`}
          >
            {currentQ.difficulty}
          </span>
        </div>

        {/* Options */}
        <div className="space-y-3">
          {options.map((opt) => {
            let style =
              'border-slate-200 dark:border-slate-800 hover:border-indigo-400 hover:bg-slate-50 dark:hover:bg-slate-800/60';

            if (selectedOption !== null && attemptResult !== null) {
              if (opt.key === attemptResult.correctOption) {
                style =
                  'border-emerald-500 bg-emerald-50/80 dark:bg-emerald-950/40 text-emerald-900 dark:text-emerald-200 font-semibold';
              } else if (opt.key === selectedOption && !attemptResult.isCorrect) {
                style =
                  'border-rose-500 bg-rose-50/80 dark:bg-rose-950/40 text-rose-900 dark:text-rose-200';
              } else {
                style = 'opacity-40 border-slate-200 dark:border-slate-800';
              }
            }

            return (
              <button
                key={opt.key}
                disabled={selectedOption !== null}
                onClick={() => handleSelectOption(opt.key)}
                className={`w-full flex items-center justify-between p-4 rounded-2xl border text-left text-sm transition ${style}`}
              >
                <div className="flex items-center gap-3">
                  <span className="w-7 h-7 rounded-xl bg-slate-100 dark:bg-slate-800 flex items-center justify-center font-bold text-xs text-slate-700 dark:text-slate-300">
                    {opt.key}
                  </span>
                  <span className="text-slate-800 dark:text-slate-200">{opt.text}</span>
                </div>

                {selectedOption !== null && attemptResult !== null && (
                  <div>
                    {opt.key === attemptResult.correctOption && (
                      <CheckCircle2 className="w-5 h-5 text-emerald-600 dark:text-emerald-400" />
                    )}
                    {opt.key === selectedOption && !attemptResult.isCorrect && (
                      <XCircle className="w-5 h-5 text-rose-600 dark:text-rose-400" />
                    )}
                  </div>
                )}
              </button>
            );
          })}
        </div>

        {/* Feedback & Explanation Card */}
        {attemptResult && (
          <div className="p-4 rounded-2xl bg-indigo-50/60 dark:bg-indigo-950/30 border border-indigo-100 dark:border-indigo-900/50 space-y-2 animate-fadeIn">
            <div className="flex items-center gap-2">
              {attemptResult.isCorrect ? (
                <span className="text-emerald-700 dark:text-emerald-400 font-bold text-xs flex items-center gap-1.5">
                  <CheckCircle2 className="w-4 h-4" /> Correct Answer!
                </span>
              ) : (
                <span className="text-rose-700 dark:text-rose-400 font-bold text-xs flex items-center gap-1.5">
                  <XCircle className="w-4 h-4" /> Incorrect. Correct option is {attemptResult.correctOption}.
                </span>
              )}
            </div>
            {attemptResult.explanation && (
              <p className="text-xs text-slate-600 dark:text-slate-300 leading-relaxed">
                <strong>Explanation:</strong> {attemptResult.explanation}
              </p>
            )}
          </div>
        )}

        {/* Action Button */}
        {selectedOption && (
          <div className="flex justify-end pt-2">
            {currentIndex < questions.length - 1 ? (
              <button
                onClick={nextQuestion}
                className="inline-flex items-center gap-2 px-6 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-bold shadow-md shadow-indigo-600/20 transition"
              >
                <span>Next Question</span>
                <ArrowRight className="w-4 h-4" />
              </button>
            ) : (
              <Link
                to={`/chapters/${id}`}
                className="inline-flex items-center gap-2 px-6 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-700 text-white text-xs font-bold transition"
              >
                <span>Complete Practice</span>
                <CheckCircle2 className="w-4 h-4" />
              </Link>
            )}
          </div>
        )}
      </div>
    </div>
  );
};
