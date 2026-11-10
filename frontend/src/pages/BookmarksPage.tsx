import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { Bookmark, Trash2, BookOpen } from 'lucide-react';

interface BookmarkItem {
  id: number;
  itemType: string;
  itemId: number;
  createdAt: string;
}

export const BookmarksPage: React.FC = () => {
  const [bookmarks, setBookmarks] = useState<BookmarkItem[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchBookmarks = async () => {
    try {
      const res = await api.get<BookmarkItem[]>('/bookmarks');
      setBookmarks(res.data);
    } catch (err) {
      console.error('Failed to load bookmarks', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBookmarks();
  }, []);

  const handleDelete = async (id: number) => {
    try {
      await api.delete(`/bookmarks/${id}`);
      setBookmarks((prev) => prev.filter((b) => b.id !== id));
    } catch (err) {
      console.error('Failed to delete bookmark', err);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600" />
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      <div className="rounded-3xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 sm:p-8 shadow-sm">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-2xl bg-indigo-50 dark:bg-indigo-950/60 text-indigo-600 dark:text-indigo-400 flex items-center justify-center">
            <Bookmark className="w-5 h-5" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
              Saved Bookmarks ({bookmarks.length})
            </h1>
            <p className="text-xs text-slate-500">
              Your pinned MCQs, interview questions, and revision notes for rapid access
            </p>
          </div>
        </div>
      </div>

      <div className="space-y-3">
        {bookmarks.length === 0 ? (
          <div className="p-12 text-center bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800">
            <Bookmark className="w-8 h-8 text-slate-400 mx-auto mb-2" />
            <p className="text-sm font-semibold text-slate-600 dark:text-slate-300">
              No saved bookmarks yet.
            </p>
          </div>
        ) : (
          bookmarks.map((b) => (
            <div
              key={b.id}
              className="p-4 px-6 rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 flex items-center justify-between gap-4 shadow-sm"
            >
              <div className="flex items-center gap-3">
                <span className="px-2 py-0.5 rounded-full text-[10px] font-bold bg-indigo-50 text-indigo-600 dark:bg-indigo-950 dark:text-indigo-400">
                  {b.itemType}
                </span>
                <span className="text-sm font-semibold text-slate-800 dark:text-slate-200">
                  Item #{b.itemId}
                </span>
                <span className="text-xs text-slate-400">
                  Saved on {new Date(b.createdAt).toLocaleDateString()}
                </span>
              </div>

              <button
                onClick={() => handleDelete(b.id)}
                className="p-2 text-slate-400 hover:text-rose-600 transition"
              >
                <Trash2 className="w-4 h-4" />
              </button>
            </div>
          ))
        )}
      </div>
    </div>
  );
};
