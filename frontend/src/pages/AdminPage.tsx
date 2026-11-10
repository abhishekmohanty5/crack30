import React, { useEffect, useState } from 'react';
import api from '../services/api';
import {
  ShieldCheck,
  Check,
  X,
  UploadCloud,
  BarChart3,
  Users,
  BookOpen,
  FileText,
  HelpCircle,
  Clock,
  AlertCircle
} from 'lucide-react';

interface PendingItem {
  id: number;
  type: string;
  title: string;
  createdByName: string;
  createdAt: string;
  preview: string;
}

interface Analytics {
  totalUsers: number;
  totalCourses: number;
  totalMcqs: number;
  totalInterviewQuestions: number;
  totalNotes: number;
  pendingContributions: number;
  totalAttempts: number;
}

export const AdminPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'pending' | 'import' | 'analytics'>('pending');
  const [pending, setPending] = useState<PendingItem[]>([]);
  const [analytics, setAnalytics] = useState<Analytics | null>(null);
  const [loading, setLoading] = useState(true);

  // Bulk import state
  const [jsonInput, setJsonInput] = useState('');
  const [importStatus, setImportStatus] = useState<string | null>(null);

  const fetchPending = async () => {
    try {
      const res = await api.get<{ items: PendingItem[] }>('/admin/pending');
      setPending(res.data.items || []);
    } catch (err) {
      console.error('Failed to load pending', err);
    }
  };

  const fetchAnalytics = async () => {
    try {
      const res = await api.get<Analytics>('/admin/analytics');
      setAnalytics(res.data);
    } catch (err) {
      console.error('Failed to load analytics', err);
    }
  };

  useEffect(() => {
    Promise.all([fetchPending(), fetchAnalytics()]).finally(() => setLoading(false));
  }, []);

  const handleApprove = async (type: string, id: number) => {
    try {
      await api.post(`/admin/${type.toLowerCase()}/${id}/approve`, {});
      setPending((prev) => prev.filter((p) => p.id !== id));
      fetchAnalytics();
    } catch (err) {
      console.error('Approve failed', err);
    }
  };

  const handleReject = async (type: string, id: number) => {
    try {
      await api.post(`/admin/${type.toLowerCase()}/${id}/reject`, { comment: 'Rejected by admin' });
      setPending((prev) => prev.filter((p) => p.id !== id));
      fetchAnalytics();
    } catch (err) {
      console.error('Reject failed', err);
    }
  };

  const handleBulkImport = async () => {
    setImportStatus(null);
    try {
      JSON.parse(jsonInput); // test parse
      await api.post('/admin/import', { type: 'bulk', jsonData: jsonInput });
      setImportStatus('✅ Bulk import completed successfully!');
      setJsonInput('');
      fetchAnalytics();
    } catch (err: any) {
      setImportStatus('❌ Import failed: ' + (err.message || 'Invalid JSON'));
    }
  };

  return (
    <div className="space-y-6 max-w-5xl mx-auto">
      {/* Admin Header */}
      <div className="rounded-3xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 sm:p-8 shadow-sm">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-2xl bg-indigo-50 dark:bg-indigo-950/60 text-indigo-600 dark:text-indigo-400 flex items-center justify-center">
            <ShieldCheck className="w-5 h-5" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Admin Moderation Hub</h1>
            <p className="text-xs text-slate-500">
              Review student peer contributions, bulk import questions, and view platform metrics
            </p>
          </div>
        </div>

        {/* Tabs */}
        <div className="flex items-center gap-2 border-b border-slate-200 dark:border-slate-800 mt-6 -mb-2">
          {[
            { id: 'pending', label: `Pending Queue (${pending.length})`, icon: Clock },
            { id: 'import', label: 'Bulk JSON Import', icon: UploadCloud },
            { id: 'analytics', label: 'Batch Analytics', icon: BarChart3 },
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id as any)}
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

      {/* Tab: Pending Reviews */}
      {activeTab === 'pending' && (
        <div className="space-y-4">
          {pending.length === 0 ? (
            <div className="p-12 text-center bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800">
              <Check className="w-8 h-8 text-emerald-500 mx-auto mb-2" />
              <p className="text-sm font-semibold text-slate-700 dark:text-slate-300">
                All caught up! No pending contributions.
              </p>
            </div>
          ) : (
            pending.map((item) => (
              <div
                key={`${item.type}-${item.id}`}
                className="p-5 rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 flex flex-col sm:flex-row sm:items-center justify-between gap-4 shadow-sm"
              >
                <div className="space-y-1.5 max-w-xl">
                  <div className="flex items-center gap-2">
                    <span className="px-2 py-0.5 rounded-full text-[10px] font-bold bg-indigo-50 text-indigo-600 dark:bg-indigo-950 dark:text-indigo-400">
                      {item.type}
                    </span>
                    <span className="text-xs text-slate-400">
                      by {item.createdByName} • {new Date(item.createdAt).toLocaleDateString()}
                    </span>
                  </div>
                  <h3 className="font-bold text-sm text-slate-900 dark:text-white">
                    {item.title}
                  </h3>
                  {item.preview && (
                    <p className="text-xs text-slate-500 line-clamp-2">{item.preview}</p>
                  )}
                </div>

                <div className="flex items-center gap-2 shrink-0">
                  <button
                    onClick={() => handleApprove(item.type, item.id)}
                    className="flex items-center gap-1.5 px-3 py-1.5 rounded-xl bg-emerald-600 hover:bg-emerald-700 text-white text-xs font-semibold shadow-sm transition"
                  >
                    <Check className="w-3.5 h-3.5" /> Approve
                  </button>
                  <button
                    onClick={() => handleReject(item.type, item.id)}
                    className="flex items-center gap-1.5 px-3 py-1.5 rounded-xl border border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-400 hover:bg-slate-50 text-xs font-semibold transition"
                  >
                    <X className="w-3.5 h-3.5" /> Reject
                  </button>
                </div>
              </div>
            ))
          )}
        </div>
      )}

      {/* Tab: Bulk Import */}
      {activeTab === 'import' && (
        <div className="rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 p-6 space-y-4 shadow-sm">
          <div>
            <h3 className="font-bold text-base text-slate-900 dark:text-white">
              Bulk JSON Content Importer
            </h3>
            <p className="text-xs text-slate-500 mt-1">
              Paste structured JSON containing <code className="bg-slate-100 dark:bg-slate-800 px-1 py-0.5 rounded">mcqs</code>, <code className="bg-slate-100 dark:bg-slate-800 px-1 py-0.5 rounded">interviewQuestions</code>, or <code className="bg-slate-100 dark:bg-slate-800 px-1 py-0.5 rounded">notes</code> (refer to <code className="text-indigo-600">sample-import.json</code> in repo).
            </p>
          </div>

          <textarea
            rows={12}
            value={jsonInput}
            onChange={(e) => setJsonInput(e.target.value)}
            placeholder={`{\n  "mcqs": [\n    {\n      "chapterId": 1,\n      "question": "What is encapsulation?",\n      "optionA": "Hiding data",\n      "optionB": "Polymorphism",\n      "optionC": "Inheritance",\n      "optionD": "None",\n      "correctOption": "A",\n      "explanation": "Encapsulation bundles data and methods.",\n      "difficulty": "Easy"\n    }\n  ]\n}`}
            className="w-full rounded-xl border border-slate-300 dark:border-slate-700 bg-slate-50 dark:bg-slate-800/80 p-3 font-mono text-xs text-slate-800 dark:text-slate-200 focus:outline-none focus:ring-2 focus:ring-indigo-500/20"
          />

          {importStatus && (
            <div className="p-3 rounded-xl bg-slate-100 dark:bg-slate-800 text-xs font-semibold">
              {importStatus}
            </div>
          )}

          <button
            onClick={handleBulkImport}
            disabled={!jsonInput.trim()}
            className="px-5 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-bold shadow-sm transition disabled:opacity-50"
          >
            Run Import
          </button>
        </div>
      )}

      {/* Tab: Analytics */}
      {activeTab === 'analytics' && analytics && (
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
          {[
            { label: 'Registered Students', val: analytics.totalUsers, icon: Users },
            { label: 'Approved MCQs', val: analytics.totalMcqs, icon: BookOpen },
            { label: 'Interview Questions', val: analytics.totalInterviewQuestions, icon: HelpCircle },
            { label: 'Revision Notes', val: analytics.totalNotes, icon: FileText },
            { label: 'Pending Queue', val: analytics.pendingContributions, icon: Clock },
            { label: 'Total Quiz Attempts', val: analytics.totalAttempts, icon: BarChart3 },
          ].map((item, i) => (
            <div
              key={i}
              className="p-5 rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 space-y-2 shadow-sm"
            >
              <div className="flex items-center justify-between text-slate-400">
                <span className="text-xs font-semibold uppercase">{item.label}</span>
                <item.icon className="w-4 h-4 text-indigo-500" />
              </div>
              <div className="text-2xl font-black text-slate-900 dark:text-white">
                {item.val}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
