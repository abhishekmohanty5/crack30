import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { TrendingUp, CheckCircle2, AlertTriangle, BookOpen, Clock } from 'lucide-react';

interface CourseProg {
  courseId: number;
  courseTitle: string;
  totalChapters: number;
  revisedChapters: number;
  progressPercent: number;
}

interface ChapterProg {
  chapterId: number;
  chapterTitle: string;
  courseName: string;
  isRevised: boolean;
  revisionCount: number;
  lastRevisedAt: string;
}

export const ProgressPage: React.FC = () => {
  const [courses, setCourses] = useState<CourseProg[]>([]);
  const [chapters, setChapters] = useState<ChapterProg[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [cRes, chRes] = await Promise.allSettled([
          api.get<CourseProg[]>('/progress/courses'),
          api.get<ChapterProg[]>('/progress/chapters'),
        ]);

        if (cRes.status === 'fulfilled') setCourses(cRes.value.data);
        if (chRes.status === 'fulfilled') setChapters(chRes.value.data);
      } catch (err) {
        console.error('Failed to load progress', err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600" />
      </div>
    );
  }

  const totalRevised = courses.reduce((acc, c) => acc + c.revisedChapters, 0);
  const totalChapters = courses.reduce((acc, c) => acc + c.totalChapters, 0);
  const overallPct = totalChapters > 0 ? Math.round((totalRevised / totalChapters) * 100) : 0;

  return (
    <div className="max-w-5xl mx-auto space-y-6">
      {/* Overview Stat Banner */}
      <div className="rounded-3xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 sm:p-8 shadow-sm flex flex-col sm:flex-row sm:items-center justify-between gap-6">
        <div>
          <span className="text-xs font-bold uppercase tracking-wider text-indigo-600 dark:text-indigo-400">
            Personal Tracking
          </span>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white mt-1">
            Curriculum Revision Progress
          </h1>
          <p className="text-xs text-slate-500 mt-1">
            {totalRevised} of {totalChapters} chapters marked as revised
          </p>
        </div>

        <div className="flex items-center gap-4 bg-indigo-50 dark:bg-indigo-950/40 p-4 px-6 rounded-2xl border border-indigo-100 dark:border-indigo-900/60">
          <div className="text-3xl font-black text-indigo-600 dark:text-indigo-400">
            {overallPct}%
          </div>
          <div className="text-xs font-semibold text-slate-600 dark:text-slate-300">
            Total<br />Coverage
          </div>
        </div>
      </div>

      {/* Per Course Progress Breakdown */}
      <div className="space-y-4">
        <h2 className="text-lg font-bold text-slate-900 dark:text-white">
          Courses Progress Breakdown
        </h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {courses.map((c) => (
            <div
              key={c.courseId}
              className="p-5 rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 space-y-3 shadow-sm"
            >
              <div className="flex items-start justify-between">
                <h3 className="font-bold text-sm text-slate-900 dark:text-white">
                  {c.courseTitle}
                </h3>
                <span className="text-xs font-bold text-indigo-600 dark:text-indigo-400">
                  {c.progressPercent}%
                </span>
              </div>

              <div className="h-1.5 w-full bg-slate-100 dark:bg-slate-800 rounded-full overflow-hidden">
                <div
                  className="h-full bg-indigo-600 transition-all duration-300"
                  style={{ width: `${c.progressPercent}%` }}
                />
              </div>

              <div className="flex justify-between text-xs text-slate-400">
                <span>{c.revisedChapters} revised</span>
                <span>{c.totalChapters} total chapters</span>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Recently Revised Chapters */}
      {chapters.length > 0 && (
        <div className="space-y-4">
          <h2 className="text-lg font-bold text-slate-900 dark:text-white">
            Revision Activity History
          </h2>
          <div className="rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 divide-y divide-slate-100 dark:divide-slate-800">
            {chapters.map((ch) => (
              <div
                key={ch.chapterId}
                className="p-4 px-6 flex items-center justify-between text-xs"
              >
                <div>
                  <div className="font-bold text-slate-800 dark:text-slate-200">
                    {ch.chapterTitle}
                  </div>
                  <div className="text-slate-400 text-[11px]">{ch.courseName}</div>
                </div>

                <div className="text-right">
                  <span className="font-semibold text-emerald-600 dark:text-emerald-400">
                    Revised ({ch.revisionCount}x)
                  </span>
                  {ch.lastRevisedAt && (
                    <div className="text-slate-400 text-[10px]">
                      {new Date(ch.lastRevisedAt).toLocaleDateString()}
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
