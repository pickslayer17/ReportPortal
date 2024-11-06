import { TestReviewVm} from './TestVmProps';
import { UserVm } from './UserVmProps';
import { EditTestReviewMode } from './../enums/EditTestReviewMode'

export interface EditTestReviewModalProps {
    isOpen: boolean;
    onClose: () => void;
    users: UserVm[];
    testReviews: TestReviewVm[];
    editMode: EditTestReviewMode;
}