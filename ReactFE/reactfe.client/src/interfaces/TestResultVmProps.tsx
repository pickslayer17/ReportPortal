import { testOutcome } from "../enums/testOutcome";
export interface TestResultVm {
    id: number;
    testId: number;
    errorMessage: string;
    stackTrace: string;
    screenShot: string; // This will be a base64 string for the image
    testOutcome: testOutcome
}