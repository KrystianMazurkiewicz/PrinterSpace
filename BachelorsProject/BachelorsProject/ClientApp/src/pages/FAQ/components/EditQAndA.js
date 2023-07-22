import React, { useEffect, useState } from "react";
import Modal from "components/Modal";

export default function EditQAndA({ id, question, answer, onEdit }) {
  const [isOpen, setIsOpen] = useState(false);
  const [stateQuestion, setStateQuestion] = useState(question);
  const [stateAnswer, setStateAnswer] = useState(answer);

  useEffect(() => {
    setStateQuestion(question);
    setStateAnswer(answer);
  }, [question, answer]);

  const openModal = () => setIsOpen(true);
  const closeModal = () => setIsOpen(false);

  function handleSubmit(e) {
    e.preventDefault();
    onEdit(id, stateQuestion, stateAnswer);
    closeModal();
  }

  return (
    <>
      <button onClick={openModal} className="button update-color">
        Edit Question
      </button>
      <Modal isOpen={isOpen} closeModal={closeModal}>
        <h2>Edit a Q&A</h2>
        <form onSubmit={handleSubmit}>
          <textarea
            required
            value={stateQuestion}
            onChange={(e) => setStateQuestion(e.target.value)}
            aria-label="Edit Question"
          ></textarea>
          <textarea
            required
            value={stateAnswer}
            onChange={(e) => setStateAnswer(e.target.value)}
            aria-label="Edit Answer"
          ></textarea>
          <button className="button update-color" type="submit">
            Update data
          </button>
        </form>
        <button onClick={closeModal} className="button danger-color">
          Cancel
        </button>
      </Modal>
    </>
  );
}
