import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../services/api';
import { Search, BookOpen, ArrowRight } from 'lucide-react';

interface SearchResult {
  type: string;
  id: number;
  title: string;
  snippet: string;
  chapterId: number;
  chapterTitle: string;
}

export const SearchPage: React.FC = () => {
  const [query, setQuery] = useState('');
  const [type, setType] = useState<string>('');
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!query.trim()) return;

    setLoading(true);
    setHasSearched(true);
    try {
      const res = await api.get<SearchResult[]>('/search', {
        params: { q: query, type: type || undefined },
      });
      setResults(res.data);
    } catch (err) {
      console.error('Search failed', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      <div className="rounded-3xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 sm:p-8 shadow-sm space-y-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
            Curriculum Global Search
          </h1>
          <p className="text-xs text-slate-500 mt-1">
            Search across MCQs, interview questions, and revision notes
          </p>
        </div>

        <form onSubmit={handleSearch} className="flex flex-col sm:flex-row gap-3">
          <div className="relative flex-1">
            <Search className="w-4 h-4 text-slate-400 absolute left-3.5 top-3.5" />
            <input
              type="text"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder="Search concepts, keywords, questions (e.g. LINQ, inheritance, async)..."
              className="w-full pl-10 pr-4 py-2.5 rounded-xl border border-slate-300 dark:border-slate-700 bg-slate-50 dark:bg-slate-800 text-xs text-slate-900 dark:text-white outline-none focus:ring-2 focus:ring-indigo-500/20"
            />
          </div>

          <select
            value={type}
            onChange={(e) => setType(e.target.value)}
            className="rounded-xl border border-slate-300 dark:border-slate-700 bg-slate-50 dark:bg-slate-800 px-3 py-2.5 text-xs text-slate-700 dark:text-slate-300 outline-none"
          >
            <option value="">All Types</option>
            <option value="mcq">MCQs</option>
            <option value="interview">Interview Q&As</option>
            <option value="note">Notes</option>
          </select>

          <button
            type="submit"
            className="px-6 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-bold transition shadow-sm"
          >
            Search
          </button>
        </form>
      </div>

      {loading && (
        <div className="flex items-center justify-center py-12">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600" />
        </div>
      )}

      {hasSearched && !loading && (
        <div className="space-y-3">
          <div className="text-xs font-semibold text-slate-400">
            Found {results.length} results
          </div>

          {results.length === 0 ? (
            <div className="p-12 text-center bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800">
              <p className="text-sm text-slate-500">No matching content found for "{query}".</p>
            </div>
          ) : (
            results.map((r, i) => (
              <div
                key={i}
                className="p-5 rounded-2xl border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 space-y-2 shadow-sm"
              >
                <div className="flex items-center gap-2">
                  <span className="px-2 py-0.5 rounded-full text-[10px] font-bold bg-indigo-50 text-indigo-600 dark:bg-indigo-950 dark:text-indigo-400">
                    {r.type}
                  </span>
                  <span className="text-xs text-slate-400 font-medium">
                    in {r.chapterTitle}
                  </span>
                </div>
                <h3 className="font-bold text-sm text-slate-800 dark:text-slate-200">
                  {r.title}
                </h3>
                <p className="text-xs text-slate-500 line-clamp-2">{r.snippet}</p>

                <div className="pt-2">
                  <Link
                    to={`/chapters/${r.chapterId}`}
                    className="inline-flex items-center gap-1 text-xs font-semibold text-indigo-600 dark:text-indigo-400 hover:underline"
                  >
                    <span>Open Chapter</span>
                    <ArrowRight className="w-3.5 h-3.5" />
                  </Link>
                </div>
              </div>
            ))
          )}
        </div>
      )}
    </div>
  );
};
