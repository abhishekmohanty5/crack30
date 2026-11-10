import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../services/api';
import { Course } from '../types/course';
import {
  Calendar,
  Clock,
  CheckCircle2,
  AlertTriangle,
  ArrowRight,
  BookOpen,
  Award,
  Sparkles,
  BarChart3
} from 'lucide-react';

interface HiringStage {
  id: number;
  batchOrCompany: string;
  title: string;
  stageDate: string;
  sortOrder: number;
  daysRemaining: number;
}

interface WeakArea {
  chapterId: number;
  chapterTitle: string;
  courseName: string;
  totalAttempts: number;
  correctAttempts: number;
  accuracyPercent: number;
}

export const DashboardPage: React.FC = () => {
  const [courses, setCourses] = useState<Course[]>([]);
  const [stages, setStages] = useState<HiringStage[]>([]);
  const [weakAreas, setWeakAreas] = useState<WeakArea[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [coursesRes, stagesRes, weakRes] = await Promise.allSettled([
          api.get<Course[]>('/courses'),
          api.get<HiringStage[]>('/hiringstages'),
          api.get<WeakArea[]>('/progress/weak-areas'),
        ]);

        if (coursesRes.status === 'fulfilled') setCourses(coursesRes.value.data);
        if (stagesRes.status === 'fulfilled') setStages(stagesRes.value.data);
        if (weakRes.status === 'fulfilled') setWeakAreas(weakRes.value.data);
      } catch (err) {
        console.error('Failed to load dashboard data', err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const nextStage = stages.find((s) => s.daysRemaining >= 0) || stages[0];

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600" />
      </div>
    );
  }

  return (
    <div className="space-y-8">
      {/* Hiring Countdown Hero Card */}
      {nextStage && (
        <div className="relative overflow-hidden rounded-3xl bg-gradient-to-r from-indigo-700 via-indigo-600 to-blue-600 p-6 sm:p-8 text-white shadow-xl shadow-indigo-600/20">
          <div className="relative z-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
            <div className="space-y-2">
              <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-white/10 text-white text-xs font-semibold backdrop-blur-md">
                <Calendar className="w-3.5 h-3.5" />
                Hiring Milestone Tracker
              </div>
              <h1 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
                {nextStage.title}
              </h1>
              <p className="text-indigo-100 text-sm max-w-xl">
                Scheduled for {nextStage.stageDate} • {nextStage.batchOrCompany}
              </p>
            </div>

            <div className="flex items-center gap-4 bg-white/10 backdrop-blur-md p-4 rounded-2xl border border-white/15 self-start md:self-auto">
              <div className="text-center px-3">
                <span className="block text-3xl sm:text-4xl font-black text-amber-300">
                  {nextStage.daysRemaining}
                </span>
                <span className="text-[11px] uppercase tracking-wider text-indigo-100 font-semibold">
                  Days Left
                </span>
              </div>
              <div className="h-10 w-px bg-white/20" />
              <div className="text-xs text-indigo-100 space-y-1">
                <div className="font-semibold text-white">Focus Daily Plan:</div>
                <div>Revise 2 chapters + 20 MCQs</div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Weak Areas Banner if any */}
      {weakAreas.length > 0 && (
        <div className="rounded-2xl border border-amber-200 dark:border-amber-900/50 bg-amber-50/70 dark:bg-amber-950/20 p-5">
          <div className="flex items-center gap-2 text-amber-800 dark:text-amber-400 font-semibold text-sm mb-3">
            <AlertTriangle className="w-4 h-4" />
            Recommended Chapters (Lowest MCQ Accuracy)
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
            {weakAreas.slice(0, 3).map((area) => (
              <Link
                key={area.chapterId}
                to={`/chapters/${area.chapterId}`}
                className="flex items-center justify-between p-3 rounded-xl bg-white dark:bg-slate-900 border border-amber-200/60 dark:border-amber-900/40 hover:border-amber-400 transition"
              >
                <div>
                  <div className="text-xs font-semibold text-slate-800 dark:text-slate-200">
                    {area.chapterTitle}
                  </div>
                  <div className="text-[11px] text-slate-500">{area.courseName}</div>
                </div>
                <span className="text-xs font-bold text-rose-600 dark:text-rose-400">
                  {area.accuracyPercent}%
                </span>
              </Link>
            ))}
          </div>
        </div>
      )}

      {/* Curriculum Courses Grid */}
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-xl font-bold text-slate-900 dark:text-white">
              Curriculum Courses
            </h2>
            <p className="text-sm text-slate-500">
              12 core technical modules mapped to your hiring assessment
            </p>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
          {courses.map((course) => (
            <div
              key={course.id}
              className="group flex flex-col justify-between rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 p-5 hover:shadow-lg hover:border-indigo-300 dark:hover:border-indigo-800 transition duration-200"
            >
              <div className="space-y-3">
                <div className="flex items-start justify-between gap-3">
                  <div className="w-10 h-10 rounded-xl bg-indigo-50 dark:bg-indigo-950/60 border border-indigo-100 dark:border-indigo-900/60 flex items-center justify-center text-indigo-600 dark:text-indigo-400">
                    <BookOpen className="w-5 h-5" />
                  </div>
                  <span className="text-xs font-semibold text-slate-500 flex items-center gap-1">
                    <Clock className="w-3.5 h-3.5" />
                    {course.hours} hrs
                  </span>
                </div>

                <div>
                  <h3 className="font-bold text-base text-slate-900 dark:text-white group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition">
                    {course.title}
                  </h3>
                  <p className="mt-1 text-xs text-slate-500 line-clamp-2 leading-relaxed">
                    {course.description || 'Comprehensive revision curriculum modules.'}
                  </p>
                </div>
              </div>

              <div className="mt-5 pt-4 border-t border-slate-100 dark:border-slate-800/80 space-y-3">
                <div className="space-y-1">
                  <div className="flex justify-between text-xs font-medium">
                    <span className="text-slate-500">Progress</span>
                    <span className="text-indigo-600 dark:text-indigo-400 font-semibold">
                      {course.progressPercent}%
                    </span>
                  </div>
                  <div className="h-1.5 w-full rounded-full bg-slate-100 dark:bg-slate-800 overflow-hidden">
                    <div
                      className="h-full rounded-full bg-indigo-600 transition-all duration-300"
                      style={{ width: `${course.progressPercent}%` }}
                    />
                  </div>
                </div>

                <Link
                  to={`/courses/${course.id}`}
                  className="w-full flex items-center justify-center gap-2 py-2 px-3 rounded-xl text-xs font-semibold text-indigo-600 dark:text-indigo-400 bg-indigo-50 dark:bg-indigo-950/40 hover:bg-indigo-100 dark:hover:bg-indigo-900/60 transition"
                >
                  <span>Explore Chapters</span>
                  <ArrowRight className="w-3.5 h-3.5" />
                </Link>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
