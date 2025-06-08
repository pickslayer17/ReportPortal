import React, { useState, useEffect } from 'react';
import { EditTestReviewModalProps } from './interfaces/EditTestReviewModalProps';
import { EditTestReviewMode } from './enums/EditTestReviewMode'
import { TestReviewOutcome } from './interfaces/TestVmProps';
import { putWithToken } from './helpers/api';

const EditTestReviewModal: React.FC<EditTestReviewModalProps> = ({
    isOpen,
    onClose,
    users,
    testReviews,
    editMode
}) => {
    const [selectedOutcome, setSelectedOutcome] = useState<TestReviewOutcome | null>(null);
    const [selectedReviewerId, setSelectedReviewerId] = useState<number | null>(null);
    const [comment, setComment] = useState<string>('');
    const [bugId, setBugId] = useState<number | null>(null);

    useEffect(() => {
        if (isOpen) {
            setSelectedOutcome(null);
            setSelectedReviewerId(null);
            setComment('');
            setBugId(null);
            if (testReviews.length == 1) {
                let tr = testReviews[0];
                setSelectedOutcome(tr.testReviewOutcome);
                setSelectedReviewerId(tr.reviewerId);
                setComment(tr.comments);
                setBugId(tr.productBug);
            } 
        }
    }, [isOpen]);

    const handleUpdate = async () => {
        const updatedTestReviews = testReviews.map(testReview => ({
            ...testReview,
            ...(editMode === EditTestReviewMode.outcome || editMode === EditTestReviewMode.all ? { testReviewOutcome: selectedOutcome } : {}),
            ...(editMode === EditTestReviewMode.reviewer || editMode === EditTestReviewMode.all ? { reviewerId: selectedReviewerId } : {}),
            ...(editMode === EditTestReviewMode.comments || editMode === EditTestReviewMode.all ? { comments: comment } : {}),
            ...(selectedOutcome === TestReviewOutcome.ProductBug && bugId !== null ? { productBug: bugId } : {}), // Add bugId for ProductBug outcome
        }));

        try {
            for (const review of updatedTestReviews) {
                const url =
                    editMode === EditTestReviewMode.reviewer ? `api/TestReviewManagement/TestReview/${review.id}/UpdateReviewer`
                        : editMode === EditTestReviewMode.outcome ? `api/TestReviewManagement/TestReview/${review.id}/UpdateOutcome`
                            : editMode === EditTestReviewMode.comments ? `api/TestReviewManagement/TestReview/${review.id}/UpdateComments`
                                : `api/TestReviewManagement/UpdateTestReview`; // Fall back to default

                const result = await putWithToken(url, review);
                console.log('Update successful:', result);
            }
        } catch (error) {
            console.error('Error updating test reviews:', error);
        } finally {
            onClose();
        }
    };

    // Убираем modal-content/modal-small, возвращаем только контент
    return (
        <>
            <h2>Edit Test Reviews</h2>
            {(editMode === EditTestReviewMode.outcome || editMode === EditTestReviewMode.all) && (
                <div>
                    <label htmlFor="outcome">Outcome:</label>
                    <select
                        id="outcome"
                        value={selectedOutcome !== null ? selectedOutcome : ''}
                        onChange={(e) => setSelectedOutcome(Number(e.target.value) as TestReviewOutcome)}
                        style={{ width: '100%' }}
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
                    {(selectedOutcome === TestReviewOutcome.ProductBug) && (
                        <div>
                            <label htmlFor="bugId">Bug Id:</label>
                            <input
                                type="number"
                                id="bugId"
                                value={bugId !== null ? bugId : ''}
                                onChange={(e) => setBugId(Number(e.target.value))}
                                placeholder="Enter Bug Id"
                                style={{ width: '100%' }}
                            />
                        </div>
                    )}
                </div>
            )}

            {(editMode === EditTestReviewMode.reviewer || editMode === EditTestReviewMode.all) && (
                <div>
                    <label htmlFor="reviewer">Reviewer:</label>
                    <select
                        id="reviewer"
                        value={selectedReviewerId !== null ? String(selectedReviewerId) : ''}
                        onChange={(e) => setSelectedReviewerId(e.target.value ? Number(e.target.value) : null)}
                        style={{ width: '100%' }}
                    >
                        <option value="">Select Reviewer</option>
                        {users.map(user => (
                            <option key={user.id} value={user.id}>
                                {user.email}
                            </option>
                        ))}
                    </select>
                </div>
            )}

            {(editMode === EditTestReviewMode.comments || editMode === EditTestReviewMode.all) && (
                <div>
                    <label htmlFor="comment">Comments:</label>
                    <textarea
                        id="comment"
                        value={comment}
                        onChange={(e) => setComment(e.target.value)}
                        placeholder="Enter your comments here"
                        rows={4}
                        style={{ width: '100%' }}
                    />
                </div>
            )}

            <div className="modal-actions">
                <button className="update-tests-button" onClick={handleUpdate}>Update</button>
                <button className="go-back-button" onClick={onClose}>Cancel</button>
            </div>
        </>
    );
};

export default EditTestReviewModal;