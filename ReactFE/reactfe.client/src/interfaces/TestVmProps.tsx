import { TestResultVm } from './TestResultVmProps';

export interface TestVm {
    id: number;
    folderId: number;
    path: string;
    runId: number;
    name: string;
    testReview: TestReviewVm,
    testResults: TestResultVm[]
}

export interface TestReviewVm {
    id: number;
    testId: number;
    reviewerId: number;
    comments: string;
    testReviewOutcome: TestReviewOutcome;
    productBug: number;
}
export enum TestReviewOutcome {
    ToInvestigate = 0,
    NotRepro = 1,
    ProductBug = 2,
}