import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { LogOut, User, BookOpen, Menu, Sparkles } from 'lucide-react';

interface NavbarProps {
  onToggleSidebar?: () => void;
}

export const Navbar: React.FC<NavbarProps> = ({ onToggleSidebar }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <header className="sticky top-0 z-40 w-full border-b border-slate-200 dark:border-slate-800 bg-white/80 dark:bg-slate-900/80 backdrop-blur-md">
      <div className="flex h-16 items-center justify-between px-4 sm:px-6">
        <div className="flex items-center gap-3">
          <button
            onClick={onToggleSidebar}
            className="md:hidden p-2 rounded-lg text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800"
          >
            <Menu className="w-5 h-5" />
          </button>
          <Link to="/" className="flex items-center gap-2">
            <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-indigo-500 to-blue-600 flex items-center justify-center text-white shadow-md shadow-indigo-500/20">
              <BookOpen className="w-5 h-5" />
            </div>
            <div className="flex flex-col">
              <span className="font-bold text-lg leading-tight bg-gradient-to-r from-indigo-600 to-blue-600 bg-clip-text text-transparent dark:from-indigo-400 dark:to-blue-400">
                Crack30
              </span>
              <span className="text-[10px] tracking-wider text-slate-400 uppercase font-semibold">
                PrepHub
              </span>
            </div>
          </Link>
        </div>

        <div className="flex items-center gap-3">
          {user ? (
            <div className="flex items-center gap-3">
              <div className="hidden sm:flex flex-col text-right">
                <span className="text-sm font-semibold text-slate-800 dark:text-slate-100">
                  {user.name}
                </span>
                <span className="text-xs text-indigo-600 dark:text-indigo-400 font-medium">
                  {user.role} {user.batch ? `• ${user.batch}` : ''}
                </span>
              </div>
              <div className="w-9 h-9 rounded-full bg-indigo-100 dark:bg-indigo-950/60 border border-indigo-200 dark:border-indigo-800 flex items-center justify-center text-indigo-600 dark:text-indigo-300 font-semibold text-sm">
                {user.name ? user.name.charAt(0).toUpperCase() : 'U'}
              </div>
              <button
                onClick={handleLogout}
                title="Sign out"
                className="p-2 rounded-lg text-slate-400 hover:text-rose-600 hover:bg-rose-50 dark:hover:bg-rose-950/30 transition-colors"
              >
                <LogOut className="w-4 h-4" />
              </button>
            </div>
          ) : (
            <div className="flex items-center gap-2">
              <Link
                to="/login"
                className="px-3.5 py-1.5 text-sm font-medium text-slate-700 dark:text-slate-200 hover:text-indigo-600 transition"
              >
                Sign in
              </Link>
              <Link
                to="/register"
                className="px-3.5 py-1.5 text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 rounded-lg shadow-sm transition"
              >
                Register
              </Link>
            </div>
          )}
        </div>
      </div>
    </header>
  );
};
