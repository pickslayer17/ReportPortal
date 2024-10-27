import React, { useState } from 'react';
import Modal from './Modal';
import { EditTestReviewModalProps } from './interfaces/EditTestReviewModalProps';
import { TestReviewOutcome } from './interfaces/TestVmProps';
import { putWithToken } from './helpers/api'; // Make sure this path is correct

const EditTestReviewModal: React.FC<EditTestReviewModalProps> = ({
    isOpen,
    onClose,
    users,
    testReviews,
}) => {
    const [selectedOutcome, setSelectedOutcome] = useState<TestReviewOutcome | null>(null);
    const [selectedReviewerId, setSelectedReviewerId] = useState<number | null>(null);

    const handleUpdate = async () => {
        const updatedTestReviews = testReviews.map(testReview => ({
            ...testReview,
            testReviewOutcome: selectedOutcome !== null ? selectedOutcome : testReview.testReviewOutcome,
            reviewerId: selectedReviewerId !== null ? selectedReviewerId : testReview.reviewerId,
        }));

        try {
            for (var i = 0; i < updatedTestReviews.length; i++) {

                var review = updatedTestReviews[i];
                const result = await putWithToken(`api/TestReviewManagement/UpdateTestReview`, review);
                console.log('Update successful:', result);

            }
        } catch (error) {
            console.error('Error updating test reviews:', error);
        } finally {
        onClose();
        }
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose}>
            <div>
                <h2>Edit Test Reviews</h2>
                <div>
                    <label htmlFor="outcome">Outcome:</label>
                    <select
                        id="outcome"
                        value={selectedOutcome !== null ? selectedOutcome : ''}
                        onChange={(e) => setSelectedOutcome(Number(e.target.value) as TestReviewOutcome)}
                    >
                        <option value="">Select Outcome</option>
                        {Object.keys(TestReviewOutcome)
                            .filter(key => isNaN(Number(key)))
                            .map(key => (
                                <option key={key} value={TestReviewOutcome[key as keyof typeof TestReviewOutcome]}>
                                    {key}
                                </option>
                            ))}
                    </select>
                </div>
                <div>
                    <label htmlFor="reviewer">Reviewer:</label>
                    <select
                        id="reviewer"
                        value={selectedReviewerId !== null ? String(selectedReviewerId) : ''}
                        onChange={(e) => setSelectedReviewerId(e.target.value ? Number(e.target.value) : null)}
                    >
                        <option value="">Select Reviewer</option>
                        {users.map(user => (
                            <option key={user.id} value={user.id}>
                                {user.email}
                            </option>
                        ))}
                    </select>
                </div>
                <button onClick={handleUpdate}>Update</button>
            </div>
        </Modal>
    );
};

export default EditTestReviewModal;