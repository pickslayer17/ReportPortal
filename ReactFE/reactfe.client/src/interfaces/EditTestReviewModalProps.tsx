import { TestReviewVm} from './TestVmProps';
import { UserVm } from './UserVmProps';

export interface EditTestReviewModalProps {
    isOpen: boolean;
    onClose: () => void;
    users: UserVm[];
    testReviews: TestReviewVm[];
}