export interface Course {
  id: string;
  title: string;
  description: string;
  hours: number;
  sortOrder: number;
  progressPercent: number;
}

export interface CourseDetail extends Course {
  topics: Topic[];
}

export interface Topic {
  id: string;
  title: string;
  chapters: ChapterSummary[];
}

export interface ChapterSummary {
  id: string;
  title: string;
  isRevised: boolean;
  revisionCount: number;
}

export interface Chapter {
  id: string;
  title: string;
  topicTitle: string;
  courseTitle: string;
  noteCount: number;
  mcqCount: number;
  interviewQuestionCount: number;
  isRevised: boolean;
  revisionCount: number;
  lastRevisedAt: string | null;
}

export interface Note {
  id: string;
  title: string;
  markdownBody: string;
  createdAt: string;
}
