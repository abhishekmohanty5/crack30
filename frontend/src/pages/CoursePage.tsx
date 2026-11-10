import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../services/api';
import { CourseDetail } from '../types/course';
import {
  BookOpen,
  CheckCircle2,
  ChevronDown,
  ChevronRight,
  Clock,
  Layers,
  HelpCircle,
  FileText,
  Code2,
  ArrowLeft
} from 'lucide-react';

export const CoursePage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [course, setCourse] = useState<CourseDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [openTopics, setOpenTopics] = useState<Record<string, boolean>>({});

  useEffect(() => {
    const fetchCourse = async () => {
      try {
        const res = await api.get<CourseDetail>(`/courses/${id}`);
        setCourse(res.data);
        // default all open
        const openMap: Record<string, boolean> = {};
        res.data.topics?.forEach((t) => {
          openMap[t.id] = true;
        });
        setOpenTopics(openMap);
      } catch (err) {
        console.error('Failed to load course', err);
      } finally {
        setLoading(false);
      }
    };

    fetchCourse();
  }, [id]);

  const toggleTopic = (topicId: string) => {
    setOpenTopics((prev) => ({ ...prev, [topicId]: !prev[topicId] }));
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600" />
      </div>
    );
  }

  if (!course) {
    return (
      <div className="text-center py-12">
        <p className="text-slate-500">Course not found.</p>
        <Link to="/" className="mt-3 text-indigo-600 font-semibold inline-block">
          Back to Dashboard
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-5xl mx-auto">
      <Link
        to="/"
        className="inline-flex items-center gap-2 text-xs font-semibold text-slate-500 hover:text-indigo-600 transition"
      >
        <ArrowLeft className="w-3.5 h-3.5" />
        Back to All Courses
      </Link>

      {/* Course Header Banner (Algomaster Style) */}
      <div className="rounded-3xl bg-gradient-to-br from-slate-900 via-slate-800 to-indigo-950 p-6 sm:p-8 text-white border border-slate-700/60 shadow-xl">
        <div className="space-y-3">
          <div className="flex items-center gap-3">
            <span className="px-3 py-1 rounded-full bg-indigo-500/20 text-indigo-300 border border-indigo-400/30 text-xs font-bold uppercase tracking-wider">
              {course.hours} Hours Curriculum
            </span>
            <span className="text-slate-400 text-xs flex items-center gap-1">
              <Layers className="w-3.5 h-3.5" />
              {course.topics?.length || 0} Topics
            </span>
          </div>

          <h1 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
            {course.title}
          </h1>

          <p className="text-slate-300 text-sm max-w-2xl leading-relaxed">
            {course.description}
          </p>
        </div>
      </div>

      {/* Topics & Chapters List */}
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-bold text-slate-900 dark:text-white">
            Curriculum Modules
          </h2>
          <span className="text-xs text-slate-500">
            Click any chapter to view notes, interview Q&As, and MCQs
          </span>
        </div>

        <div className="space-y-3">
          {course.topics?.map((topic, index) => {
            const isOpen = openTopics[topic.id] ?? true;
            return (
              <div
                key={topic.id}
                className="rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 overflow-hidden shadow-sm"
              >
                <button
                  onClick={() => toggleTopic(topic.id)}
                  className="w-full flex items-center justify-between p-4.5 px-6 text-left hover:bg-slate-50/80 dark:hover:bg-slate-800/40 transition"
                >
                  <div className="flex items-center gap-3">
                    <span className="w-6 h-6 rounded-lg bg-indigo-50 dark:bg-indigo-950/60 border border-indigo-100 dark:border-indigo-900 flex items-center justify-center text-xs font-bold text-indigo-600 dark:text-indigo-400">
                      {index + 1}
                    </span>
                    <span className="font-bold text-base text-slate-800 dark:text-slate-200">
                      {topic.title}
                    </span>
                    <span className="text-xs text-slate-400 font-medium ml-1">
                      ({topic.chapters?.length || 0} chapters)
                    </span>
                  </div>
                  {isOpen ? (
                    <ChevronDown className="w-5 h-5 text-slate-400" />
                  ) : (
                    <ChevronRight className="w-5 h-5 text-slate-400" />
                  )}
                </button>

                {isOpen && (
                  <div className="divide-y divide-slate-100 dark:divide-slate-800/80 border-t border-slate-100 dark:border-slate-800/80 bg-slate-50/50 dark:bg-slate-900/50">
                    {topic.chapters?.map((chapter) => (
                      <Link
                        key={chapter.id}
                        to={`/chapters/${chapter.id}`}
                        className="flex items-center justify-between px-6 py-3.5 hover:bg-white dark:hover:bg-slate-800 transition group"
                      >
                        <div className="flex items-center gap-3">
                          <div
                            className={`w-5 h-5 rounded-full flex items-center justify-center text-xs ${
                              chapter.isRevised
                                ? 'bg-emerald-100 text-emerald-600 dark:bg-emerald-950 dark:text-emerald-400'
                                : 'bg-slate-200 text-slate-400 dark:bg-slate-800 dark:text-slate-600'
                            }`}
                          >
                            <CheckCircle2 className="w-3.5 h-3.5" />
                          </div>
                          <div>
                            <span className="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition">
                              {chapter.title}
                            </span>
                            {chapter.isRevised && (
                              <span className="ml-2.5 inline-block text-[10px] font-semibold text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-950/40 px-2 py-0.5 rounded-full">
                                Revised ({chapter.revisionCount}x)
                              </span>
                            )}
                          </div>
                        </div>

                        <div className="flex items-center gap-3 text-xs text-slate-400">
                          <span className="group-hover:translate-x-1 transition-transform">
                            →
                          </span>
                        </div>
                      </Link>
                    ))}
                  </div>
                )}
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
};
