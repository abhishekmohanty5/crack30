import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../services/api';
import ReactMarkdown from 'react-markdown';
import { Chapter, Note } from '../types/course';
import { Mcq } from '../types/mcq';
import { InterviewQuestion } from '../types/interview';
import {
  BookOpen,
  HelpCircle,
  FileText,
  Code2,
  CheckCircle2,
  Eye,
  EyeOff,
  Sparkles,
  ArrowRight,
  Play,
  RotateCcw,
  Tag,
  ThumbsUp,
  Bookmark
} from 'lucide-react';

export const ChapterPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [chapter, setChapter] = useState<Chapter | null>(null);
  const [activeTab, setActiveTab] = useState<'notes' | 'interview' | 'mcq' | 'code'>('notes');
  const [loading, setLoading] = useState(true);

  // Content states
  const [notes, setNotes] = useState<Note[]>([]);
  const [questions, setQuestions] = useState<InterviewQuestion[]>([]);
  const [mcqs, setMcqs] = useState<Mcq[]>([]);
  const [revealedAnswers, setRevealedAnswers] = useState<Record<string, boolean>>({});

  useEffect(() => {
    const fetchChapterData = async () => {
      try {
        const [chRes, notesRes, intRes, mcqRes] = await Promise.allSettled([
          api.get<Chapter>(`/chapters/${id}`),
          api.get<Note[]>(`/notes/chapter/${id}`),
          api.get<{ items: InterviewQuestion[] }>(`/interviewquestions/chapter/${id}`),
          api.get<{ items: Mcq[] }>(`/mcqs/chapter/${id}`),
        ]);

        if (chRes.status === 'fulfilled') setChapter(chRes.value.data);
        if (notesRes.status === 'fulfilled') setNotes(notesRes.value.data);
        if (intRes.status === 'fulfilled') setQuestions(intRes.value.data.items || []);
        if (mcqRes.status === 'fulfilled') setMcqs(mcqRes.value.data.items || []);
      } catch (err) {
        console.error('Error fetching chapter', err);
      } finally {
        setLoading(false);
      }
    };

    fetchChapterData();
  }, [id]);

  const handleMarkRevised = async () => {
    try {
      const res = await api.put<Chapter>(`/chapters/${id}/revise`);
      setChapter(res.data);
    } catch (err) {
      console.error('Failed to mark revised', err);
    }
  };

  const toggleReveal = (qId: string) => {
    setRevealedAnswers((prev) => ({ ...prev, [qId]: !prev[qId] }));
  };

  const handleInterviewStatus = async (qId: string, status: 'Confident' | 'NeedRevision') => {
    try {
      await api.put(`/interviewquestions/${qId}/progress`, { status });
      setQuestions((prev) =>
        prev.map((q) => (q.id === qId ? { ...q, userStatus: status } : q))
      );
    } catch (err) {
      console.error('Failed to update status', err);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600" />
      </div>
    );
  }

  if (!chapter) {
    return (
      <div className="text-center py-12">
        <p className="text-slate-500">Chapter not found.</p>
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-5xl mx-auto">
      {/* Chapter Header Card */}
      <div className="rounded-3xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 sm:p-8 shadow-sm">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div className="space-y-1">
            <span className="text-xs font-semibold text-indigo-600 dark:text-indigo-400 uppercase tracking-wider">
              {chapter.courseTitle || 'Curriculum'}
            </span>
            <h1 className="text-2xl sm:text-3xl font-extrabold text-slate-900 dark:text-white">
              {chapter.title}
            </h1>
            <p className="text-xs text-slate-400">
              {chapter.noteCount} Notes • {chapter.interviewQuestionCount} Interview Q&As • {chapter.mcqCount} MCQs
            </p>
          </div>

          <button
            onClick={handleMarkRevised}
            className={`inline-flex items-center gap-2 px-4 py-2.5 rounded-xl text-xs font-bold transition shadow-sm ${
              chapter.isRevised
                ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-300 border border-emerald-200 dark:border-emerald-800 hover:bg-emerald-100'
                : 'bg-indigo-600 text-white hover:bg-indigo-700 shadow-indigo-600/20'
            }`}
          >
            <CheckCircle2 className="w-4 h-4" />
            <span>
              {chapter.isRevised
                ? `Revised (${chapter.revisionCount}x)`
                : 'Mark as Revised'}
            </span>
          </button>
        </div>

        {/* 4 Tabs */}
        <div className="flex items-center gap-2 border-b border-slate-200 dark:border-slate-800 mt-8 -mb-2 overflow-x-auto">
          {[
            { id: 'notes', label: 'Revision Notes', count: notes.length, icon: FileText },
            { id: 'interview', label: 'Interview Q&A', count: questions.length, icon: HelpCircle },
            { id: 'mcq', label: 'MCQs', count: mcqs.length, icon: BookOpen },
            { id: 'code', label: 'Code Practice', count: 0, icon: Code2 },
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id as any)}
              className={`flex items-center gap-2 px-4 py-3 border-b-2 text-xs font-semibold whitespace-nowrap transition ${
                activeTab === tab.id
                  ? 'border-indigo-600 text-indigo-600 dark:text-indigo-400'
                  : 'border-transparent text-slate-500 hover:text-slate-800 dark:hover:text-slate-300'
              }`}
            >
              <tab.icon className="w-4 h-4" />
              <span>{tab.label}</span>
              <span className="px-1.5 py-0.5 rounded-full text-[10px] bg-slate-100 dark:bg-slate-800 text-slate-500 font-bold">
                {tab.count}
              </span>
            </button>
          ))}
        </div>
      </div>

      {/* Tab 1: Notes */}
      {activeTab === 'notes' && (
        <div className="space-y-4">
          {notes.length === 0 ? (
            <div className="text-center py-12 bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 p-8">
              <FileText className="w-10 h-10 text-slate-400 mx-auto mb-2" />
              <p className="text-sm font-semibold text-slate-600 dark:text-slate-300">
                No notes published for this chapter yet.
              </p>
              <Link
                to="/contribute"
                className="mt-3 text-xs font-bold text-indigo-600 hover:underline inline-block"
              >
                Contribute a revision sheet →
              </Link>
            </div>
          ) : (
            notes.map((note) => (
              <div
                key={note.id}
                className="rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 p-6 sm:p-8 space-y-4 shadow-sm"
              >
                <h3 className="text-xl font-bold text-slate-900 dark:text-white border-b border-slate-100 dark:border-slate-800 pb-3">
                  {note.title}
                </h3>
                <div className="prose dark:prose-invert max-w-none text-slate-700 dark:text-slate-300 text-sm leading-relaxed">
                  <ReactMarkdown>{note.markdownBody}</ReactMarkdown>
                </div>
              </div>
            ))
          )}
        </div>
      )}

      {/* Tab 2: Interview Q&A */}
      {activeTab === 'interview' && (
        <div className="space-y-4">
          {questions.length === 0 ? (
            <div className="text-center py-12 bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 p-8">
              <HelpCircle className="w-10 h-10 text-slate-400 mx-auto mb-2" />
              <p className="text-sm font-semibold text-slate-600 dark:text-slate-300">
                No interview questions added yet.
              </p>
            </div>
          ) : (
            questions.map((q, idx) => {
              const isRevealed = revealedAnswers[q.id];
              return (
                <div
                  key={q.id}
                  className="rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 p-6 space-y-4 shadow-sm"
                >
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex items-start gap-3">
                      <span className="w-6 h-6 rounded-lg bg-indigo-50 dark:bg-indigo-950/60 text-indigo-600 dark:text-indigo-400 text-xs font-bold flex items-center justify-center shrink-0 mt-0.5">
                        {idx + 1}
                      </span>
                      <h4 className="font-bold text-base text-slate-900 dark:text-white">
                        {q.question}
                      </h4>
                    </div>

                    <div className="flex items-center gap-2 shrink-0">
                      <span
                        className={`text-[10px] font-bold px-2 py-0.5 rounded-full ${
                          q.difficulty === 'Easy'
                            ? 'bg-emerald-50 text-emerald-600 dark:bg-emerald-950 dark:text-emerald-400'
                            : q.difficulty === 'Medium'
                            ? 'bg-amber-50 text-amber-600 dark:bg-amber-950 dark:text-amber-400'
                            : 'bg-rose-50 text-rose-600 dark:bg-rose-950 dark:text-rose-400'
                        }`}
                      >
                        {q.difficulty}
                      </span>
                      <span className="text-[10px] font-bold px-2 py-0.5 rounded-full bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-400">
                        {q.frequency}
                      </span>
                    </div>
                  </div>

                  {/* Reveal Answer Toggle */}
                  <div className="pt-2">
                    <button
                      onClick={() => toggleReveal(q.id)}
                      className="inline-flex items-center gap-1.5 text-xs font-semibold text-indigo-600 dark:text-indigo-400 hover:underline"
                    >
                      {isRevealed ? (
                        <>
                          <EyeOff className="w-3.5 h-3.5" />
                          <span>Hide Model Answer</span>
                        </>
                      ) : (
                        <>
                          <Eye className="w-3.5 h-3.5" />
                          <span>Reveal Model Answer</span>
                        </>
                      )}
                    </button>

                    {isRevealed && (
                      <div className="mt-3 p-4 rounded-xl bg-slate-50 dark:bg-slate-800/70 border border-slate-200/70 dark:border-slate-700/60 text-xs leading-relaxed text-slate-800 dark:text-slate-200 animate-fadeIn">
                        <div className="font-bold text-slate-500 uppercase tracking-wider text-[10px] mb-1">
                          Model Answer:
                        </div>
                        <p>{q.modelAnswer}</p>
                      </div>
                    )}
                  </div>

                  {/* Status buttons: Confident / Need Revision */}
                  <div className="flex items-center justify-between pt-3 border-t border-slate-100 dark:border-slate-800 text-xs">
                    <span className="text-slate-400 text-[11px]">
                      Your status:{' '}
                      <strong className="text-slate-700 dark:text-slate-300">
                        {q.userStatus || 'Not assessed'}
                      </strong>
                    </span>

                    <div className="flex items-center gap-2">
                      <button
                        onClick={() => handleInterviewStatus(q.id, 'NeedRevision')}
                        className={`px-3 py-1 rounded-lg text-xs font-medium border transition ${
                          q.userStatus === 'NeedRevision'
                            ? 'bg-amber-100 border-amber-300 text-amber-800 dark:bg-amber-950 dark:border-amber-800 dark:text-amber-300'
                            : 'border-slate-200 dark:border-slate-800 hover:bg-slate-50 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400'
                        }`}
                      >
                        Need Revision
                      </button>
                      <button
                        onClick={() => handleInterviewStatus(q.id, 'Confident')}
                        className={`px-3 py-1 rounded-lg text-xs font-medium border transition ${
                          q.userStatus === 'Confident'
                            ? 'bg-emerald-100 border-emerald-300 text-emerald-800 dark:bg-emerald-950 dark:border-emerald-800 dark:text-emerald-300'
                            : 'border-slate-200 dark:border-slate-800 hover:bg-slate-50 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400'
                        }`}
                      >
                        Confident
                      </button>
                    </div>
                  </div>
                </div>
              );
            })
          )}
        </div>
      )}

      {/* Tab 3: MCQs */}
      {activeTab === 'mcq' && (
        <div className="space-y-6">
          {/* Practice & Quiz Action Cards (Algomaster style) */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="p-6 rounded-2xl bg-gradient-to-br from-indigo-500 to-blue-600 text-white shadow-md space-y-3">
              <div className="inline-flex p-2 rounded-xl bg-white/20 backdrop-blur-md">
                <Play className="w-5 h-5 text-white" />
              </div>
              <h3 className="text-lg font-bold">Practice Mode</h3>
              <p className="text-xs text-indigo-100 leading-relaxed">
                One question at a time with instant answer check, correct options, and explanations.
              </p>
              <Link
                to={`/chapters/${id}/practice`}
                className="inline-flex items-center gap-2 px-4 py-2 rounded-xl bg-white text-indigo-600 text-xs font-bold hover:bg-indigo-50 transition"
              >
                <span>Start Practice</span>
                <ArrowRight className="w-3.5 h-3.5" />
              </Link>
            </div>

            <div className="p-6 rounded-2xl bg-gradient-to-br from-purple-600 to-indigo-700 text-white shadow-md space-y-3">
              <div className="inline-flex p-2 rounded-xl bg-white/20 backdrop-blur-md">
                <RotateCcw className="w-5 h-5 text-white" />
              </div>
              <h3 className="text-lg font-bold">Chapter Quiz (10 Questions)</h3>
              <p className="text-xs text-purple-100 leading-relaxed">
                Timed assessment mode with 10 random questions, progress palette, and final score.
              </p>
              <Link
                to={`/chapters/${id}/quiz`}
                className="inline-flex items-center gap-2 px-4 py-2 rounded-xl bg-white text-purple-600 text-xs font-bold hover:bg-purple-50 transition"
              >
                <span>Take Quiz</span>
                <ArrowRight className="w-3.5 h-3.5" />
              </Link>
            </div>
          </div>

          {/* Question List Preview */}
          <div className="space-y-3">
            <h3 className="text-base font-bold text-slate-800 dark:text-slate-200">
              Questions in this Chapter ({mcqs.length})
            </h3>
            {mcqs.map((m, idx) => (
              <div
                key={m.id}
                className="p-4 rounded-xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 flex items-center justify-between gap-4"
              >
                <div className="flex items-center gap-3">
                  <span className="text-xs font-bold text-slate-400">#{idx + 1}</span>
                  <span className="text-sm font-semibold text-slate-800 dark:text-slate-200">
                    {m.question}
                  </span>
                </div>
                <span
                  className={`text-[10px] font-bold px-2 py-0.5 rounded-full shrink-0 ${
                    m.difficulty === 'Easy'
                      ? 'bg-emerald-50 text-emerald-600 dark:bg-emerald-950 dark:text-emerald-400'
                      : m.difficulty === 'Medium'
                      ? 'bg-amber-50 text-amber-600 dark:bg-amber-950 dark:text-amber-400'
                      : 'bg-rose-50 text-rose-600 dark:bg-rose-950 dark:text-rose-400'
                  }`}
                >
                  {m.difficulty}
                </span>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Tab 4: Code Practice (Phase 2 preview) */}
      {activeTab === 'code' && (
        <div className="text-center py-16 bg-white dark:bg-slate-900 rounded-3xl border border-slate-200 dark:border-slate-800 p-8 space-y-4 shadow-sm">
          <div className="w-14 h-14 rounded-2xl bg-indigo-50 dark:bg-indigo-950/60 border border-indigo-100 dark:border-indigo-900/60 flex items-center justify-center text-indigo-600 dark:text-indigo-400 mx-auto">
            <Code2 className="w-7 h-7" />
          </div>
          <div className="max-w-md mx-auto space-y-2">
            <h3 className="text-xl font-bold text-slate-900 dark:text-white">
              LeetCode-Style In-Browser Code Sandbox
            </h3>
            <p className="text-xs text-slate-500 leading-relaxed">
              Phase 2 is scheduled to introduce the Monaco in-browser code editor, language selector (C#, Java, Python, SQL), test case runner, and sandbox execution via Judge0.
            </p>
          </div>
        </div>
      )}
    </div>
  );
};
