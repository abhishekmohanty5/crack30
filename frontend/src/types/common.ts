export interface CourseProgress {
  courseId: string;
  courseTitle: string;
  totalChapters: number;
  revisedChapters: number;
  mcqAccuracy: number;
  progressPercent: number;
}

export interface WeakArea {
  chapterId: string;
  chapterTitle: string;
  courseTitle: string;
  accuracy: number;
  totalAttempts: number;
}

export interface Bookmark {
  id: string;
  itemType: 'mcq' | 'interview' | 'note';
  itemId: string;
  title: string;
  createdAt: string;
}

export interface BookmarkRequest {
  itemType: string;
  itemId: string;
}

export interface SearchResult {
  id: string;
  type: 'mcq' | 'interview' | 'note';
  title: string;
  snippet: string;
  courseTitle: string;
  chapterTitle: string;
  difficulty: string;
}

export interface SearchParams {
  q: string;
  course?: string;
  type?: string;
  difficulty?: string;
}

export interface HiringStage {
  id: string;
  title: string;
  stageDate: string;
  batchOrCompany: string;
  daysRemaining: number;
}

export interface PendingItem {
  id: string;
  type: 'mcq' | 'interview' | 'note';
  content: Record<string, unknown>;
  submittedBy: string;
  submittedAt: string;
}

export interface AdminAnalytics {
  totalUsers: number;
  totalMcqs: number;
  totalInterviewQs: number;
  totalNotes: number;
  pendingCount: number;
}

export interface ContributeMcqRequest {
  chapterId: string;
  question: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctOption: string;
  explanation: string;
  difficulty: string;
  tags: string[];
}

export interface ContributeInterviewRequest {
  chapterId: string;
  question: string;
  modelAnswer: string;
  difficulty: string;
  frequency: string;
  tags: string[];
}

export interface ContributeNoteRequest {
  chapterId: string;
  title: string;
  markdownBody: string;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}
