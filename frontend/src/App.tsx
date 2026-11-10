import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './context/AuthContext';
import { RootLayout } from './components/layout/RootLayout';
import { ProtectedRoute } from './components/layout/ProtectedRoute';

import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { DashboardPage } from './pages/DashboardPage';
import { CoursePage } from './pages/CoursePage';
import { ChapterPage } from './pages/ChapterPage';
import { McqPracticePage } from './pages/McqPracticePage';
import { McqQuizPage } from './pages/McqQuizPage';
import { ProgressPage } from './pages/ProgressPage';
import { BookmarksPage } from './pages/BookmarksPage';
import { SearchPage } from './pages/SearchPage';
import { ContributePage } from './pages/ContributePage';
import { AdminPage } from './pages/AdminPage';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

export const App: React.FC = () => {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            {/* Public Auth Routes */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            {/* Protected App Routes inside RootLayout */}
            <Route
              path="/"
              element={
                <ProtectedRoute>
                  <RootLayout />
                </ProtectedRoute>
              }
            >
              <Route index element={<DashboardPage />} />
              <Route path="courses/:id" element={<CoursePage />} />
              <Route path="chapters/:id" element={<ChapterPage />} />
              <Route path="chapters/:id/practice" element={<McqPracticePage />} />
              <Route path="chapters/:id/quiz" element={<McqQuizPage />} />
              <Route path="progress" element={<ProgressPage />} />
              <Route path="bookmarks" element={<BookmarksPage />} />
              <Route path="search" element={<SearchPage />} />
              <Route path="contribute" element={<ContributePage />} />
              <Route
                path="admin"
                element={
                  <ProtectedRoute requireAdmin>
                    <AdminPage />
                  </ProtectedRoute>
                }
              />
            </Route>

            {/* Catch-all redirect */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
};

export default App;
