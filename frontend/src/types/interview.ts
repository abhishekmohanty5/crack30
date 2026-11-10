export interface InterviewQuestion {
  id: string;
  question: string;
  modelAnswer: string;
  difficulty: 'Easy' | 'Medium' | 'Hard';
  frequency: string;
  tags: string[];
  userStatus: 'Confident' | 'NeedRevision' | null;
}

export interface InterviewProgressRequest {
  status: 'Confident' | 'NeedRevision';
}
