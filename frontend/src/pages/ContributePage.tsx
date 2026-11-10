import React, { useState } from 'react';
import api from '../services/api';
import { PlusCircle, CheckCircle2, FileText, HelpCircle, BookOpen } from 'lucide-react';

export const ContributePage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'mcq' | 'interview' | 'note'>('mcq');
  const [chapterId, setChapterId] = useState(1);
  const [success, setSuccess] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  // Form states
  const [mcqForm, setMcqForm] = useState({
    question: '',
    optionA: '',
    optionB: '',
    optionC: '',
    optionD: '',
    correctOption: 'A',
    explanation: '',
    difficulty: 'Easy',
    tags: '',
  });

  const [interviewForm, setInterviewForm] = useState({
    question: '',
    modelAnswer: '',
    difficulty: 'Medium',
    frequency: 'Common',
    tags: '',
  });

  const [noteForm, setNoteForm] = useState({
    title: '',
    markdownBody: '',
  });

  const handleMcqSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      await api.post('/contributions/mcq', { chapterId, ...mcqForm });
      setSuccess(true);
      setMcqForm({
        question: '',
        optionA: '',
        optionB: '',
        optionC: '',
        optionD: '',
        correctOption: 'A',
        explanation: '',
        difficulty: 'Easy',
        tags: '',
      });
    } catch (err) {
      console.error('Submit failed', err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleInterviewSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      await api.post('/contributions/interview', { chapterId, ...interviewForm });
      setSuccess(true);
      setInterviewForm({
        question: '',
        modelAnswer: '',
        difficulty: 'Medium',
        frequency: 'Common',
        tags: '',
      });
    } catch (err) {
      console.error('Submit failed', err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleNoteSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      await api.post('/contributions/note', { chapterId, ...noteForm });
      setSuccess(true);
      setNoteForm({ title: '', markdownBody: '' });
    } catch (err) {
      console.error('Submit failed', err);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div className="rounded-3xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 sm:p-8 shadow-sm">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-2xl bg-indigo-50 dark:bg-indigo-950/60 text-indigo-600 dark:text-indigo-400 flex items-center justify-center">
            <PlusCircle className="w-5 h-5" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
              Student Contribution Portal
            </h1>
            <p className="text-xs text-slate-500">
              Submit revision content for your batch. Items enter the review queue and appear once approved.
            </p>
          </div>
        </div>

        {/* Tab selection */}
        <div className="flex items-center gap-2 border-b border-slate-200 dark:border-slate-800 mt-6 -mb-2">
          {[
            { id: 'mcq', label: 'Submit MCQ', icon: BookOpen },
            { id: 'interview', label: 'Submit Interview Q&A', icon: HelpCircle },
            { id: 'note', label: 'Submit Revision Note', icon: FileText },
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => {
                setActiveTab(tab.id as any);
                setSuccess(false);
              }}
              className={`flex items-center gap-2 px-4 py-3 border-b-2 text-xs font-semibold transition ${
                activeTab === tab.id
                  ? 'border-indigo-600 text-indigo-600 dark:text-indigo-400'
                  : 'border-transparent text-slate-500 hover:text-slate-800'
              }`}
            >
              <tab.icon className="w-4 h-4" />
              <span>{tab.label}</span>
            </button>
          ))}
        </div>
      </div>

      {success && (
        <div className="p-4 rounded-2xl bg-emerald-50 dark:bg-emerald-950/40 border border-emerald-200 dark:border-emerald-800 text-emerald-800 dark:text-emerald-300 text-xs flex items-center gap-2">
          <CheckCircle2 className="w-4 h-4 shrink-0" />
          <span>Thank you! Your contribution has been submitted for admin review.</span>
        </div>
      )}

      {/* Forms */}
      <div className="rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 p-6 space-y-4 shadow-sm">
        <div>
          <label className="block text-xs font-bold text-slate-700 dark:text-slate-300 mb-1">
            Target Chapter ID (1 = OOP Basics, 5 = Inheritance, etc.)
          </label>
          <input
            type="number"
            min={1}
            max={50}
            value={chapterId}
            onChange={(e) => setChapterId(Number(e.target.value))}
            className="w-32 rounded-xl border border-slate-300 dark:border-slate-700 bg-slate-50 dark:bg-slate-800 px-3 py-1.5 text-xs font-bold"
          />
        </div>

        {activeTab === 'mcq' && (
          <form onSubmit={handleMcqSubmit} className="space-y-4 text-xs">
            <div>
              <label className="block font-semibold mb-1">Question Statement</label>
              <textarea
                required
                rows={3}
                value={mcqForm.question}
                onChange={(e) => setMcqForm({ ...mcqForm, question: e.target.value })}
                className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-3 bg-slate-50 dark:bg-slate-800"
                placeholder="What is the output of...?"
              />
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <div>
                <label className="block font-semibold mb-1">Option A</label>
                <input
                  required
                  type="text"
                  value={mcqForm.optionA}
                  onChange={(e) => setMcqForm({ ...mcqForm, optionA: e.target.value })}
                  className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                />
              </div>
              <div>
                <label className="block font-semibold mb-1">Option B</label>
                <input
                  required
                  type="text"
                  value={mcqForm.optionB}
                  onChange={(e) => setMcqForm({ ...mcqForm, optionB: e.target.value })}
                  className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                />
              </div>
              <div>
                <label className="block font-semibold mb-1">Option C</label>
                <input
                  required
                  type="text"
                  value={mcqForm.optionC}
                  onChange={(e) => setMcqForm({ ...mcqForm, optionC: e.target.value })}
                  className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                />
              </div>
              <div>
                <label className="block font-semibold mb-1">Option D</label>
                <input
                  required
                  type="text"
                  value={mcqForm.optionD}
                  onChange={(e) => setMcqForm({ ...mcqForm, optionD: e.target.value })}
                  className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block font-semibold mb-1">Correct Option</label>
                <select
                  value={mcqForm.correctOption}
                  onChange={(e) => setMcqForm({ ...mcqForm, correctOption: e.target.value })}
                  className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                >
                  <option value="A">A</option>
                  <option value="B">B</option>
                  <option value="C">C</option>
                  <option value="D">D</option>
                </select>
              </div>
              <div>
                <label className="block font-semibold mb-1">Difficulty</label>
                <select
                  value={mcqForm.difficulty}
                  onChange={(e) => setMcqForm({ ...mcqForm, difficulty: e.target.value })}
                  className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                >
                  <option value="Easy">Easy</option>
                  <option value="Medium">Medium</option>
                  <option value="Hard">Hard</option>
                </select>
              </div>
            </div>

            <div>
              <label className="block font-semibold mb-1">Explanation</label>
              <textarea
                rows={2}
                value={mcqForm.explanation}
                onChange={(e) => setMcqForm({ ...mcqForm, explanation: e.target.value })}
                className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-3 bg-slate-50 dark:bg-slate-800"
                placeholder="Explain why this option is correct..."
              />
            </div>

            <button
              type="submit"
              disabled={submitting}
              className="px-6 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white font-bold shadow-sm transition disabled:opacity-50"
            >
              Submit MCQ
            </button>
          </form>
        )}

        {activeTab === 'interview' && (
          <form onSubmit={handleInterviewSubmit} className="space-y-4 text-xs">
            <div>
              <label className="block font-semibold mb-1">Interview Question</label>
              <input
                required
                type="text"
                value={interviewForm.question}
                onChange={(e) => setInterviewForm({ ...interviewForm, question: e.target.value })}
                className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                placeholder="e.g. Explain difference between interface and abstract class."
              />
            </div>

            <div>
              <label className="block font-semibold mb-1">Model Answer</label>
              <textarea
                required
                rows={4}
                value={interviewForm.modelAnswer}
                onChange={(e) => setInterviewForm({ ...interviewForm, modelAnswer: e.target.value })}
                className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-3 bg-slate-50 dark:bg-slate-800"
                placeholder="Clear 3-6 sentences explaining the concept with a concise code example if relevant."
              />
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block font-semibold mb-1">Difficulty</label>
                <select
                  value={interviewForm.difficulty}
                  onChange={(e) => setInterviewForm({ ...interviewForm, difficulty: e.target.value })}
                  className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                >
                  <option value="Easy">Easy</option>
                  <option value="Medium">Medium</option>
                  <option value="Hard">Hard</option>
                </select>
              </div>
              <div>
                <label className="block font-semibold mb-1">Frequency</label>
                <select
                  value={interviewForm.frequency}
                  onChange={(e) => setInterviewForm({ ...interviewForm, frequency: e.target.value })}
                  className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                >
                  <option value="Common">Common</option>
                  <option value="Occasional">Occasional</option>
                  <option value="Rare">Rare</option>
                </select>
              </div>
            </div>

            <button
              type="submit"
              disabled={submitting}
              className="px-6 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white font-bold shadow-sm transition disabled:opacity-50"
            >
              Submit Question
            </button>
          </form>
        )}

        {activeTab === 'note' && (
          <form onSubmit={handleNoteSubmit} className="space-y-4 text-xs">
            <div>
              <label className="block font-semibold mb-1">Note Title</label>
              <input
                required
                type="text"
                value={noteForm.title}
                onChange={(e) => setNoteForm({ ...noteForm, title: e.target.value })}
                className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-2.5 bg-slate-50 dark:bg-slate-800"
                placeholder="e.g. LINQ Key Operations Quick Sheet"
              />
            </div>

            <div>
              <label className="block font-semibold mb-1">Markdown Body</label>
              <textarea
                required
                rows={8}
                value={noteForm.markdownBody}
                onChange={(e) => setNoteForm({ ...noteForm, markdownBody: e.target.value })}
                className="w-full rounded-xl border border-slate-300 dark:border-slate-700 p-3 bg-slate-50 dark:bg-slate-800 font-mono text-xs"
                placeholder="# Key Concepts\n- Concept 1\n- Concept 2"
              />
            </div>

            <button
              type="submit"
              disabled={submitting}
              className="px-6 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white font-bold shadow-sm transition disabled:opacity-50"
            >
              Submit Note
            </button>
          </form>
        )}
      </div>
    </div>
  );
};
