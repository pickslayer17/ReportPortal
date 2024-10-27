export interface TestVm {
    id: number;
    folderId: number;
    path: string;
    runId: number;
    name: string;
    testReview: TestReviewVm
}

export interface TestReviewVm {
    id: number;
    testId: number;
    reviewerId: number;
    comments: string;
    testReviewOutcome: TestReviewOutcome;
}
export enum TestReviewOutcome {
    ToInvestigate = 0,
    NotRepro = 1,
    ProductBug = 2,
}