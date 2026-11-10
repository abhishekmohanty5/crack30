export interface Mcq {
  id: string;
  question: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  difficulty: 'Easy' | 'Medium' | 'Hard';
  tags: string[];
}

export interface McqQuizItem {
  id: string;
  question: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
}

export interface McqAttemptRequest {
  chosenOption: string;
  timeTakenSec: number;
}

export interface McqAttemptResponse {
  isCorrect: boolean;
  correctOption: string;
  explanation: string;
}
