import React, { useEffect, useState } from "react";
import PlusInsideCircleIcon from "components/icons/PlusInsideCircleIcon";
import Modal from "components/Modal";

export default function AddQAndA({ onCreate }) {
  const [isOpen, setIsOpen] = useState(false);
  const [question, setQuestion] = useState("");
  const [answer, setAnswer] = useState("");

  useEffect(() => {
    if (!isOpen) {
      setQuestion("");
      setAnswer("");
    }
  }, [isOpen]);

  const openModal = () => setIsOpen(true);
  const closeModal = () => setIsOpen(false);

  function handleSubmit(event) {
    event.preventDefault();
    onCreate(question, answer);
    closeModal();
  }

  return (
    <>
      <a href="javascript:void(0);" className="list-item" onClick={openModal}>
        <div className="item-container">
          <PlusInsideCircleIcon />
          <p className="file-name">Add a new question and answer</p>
        </div>
      </a>
      <Modal isOpen={isOpen} closeModal={closeModal}>
        <h2>Add Question</h2>
        <form onSubmit={handleSubmit}>
          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="question"
              type="text"
              value={question}
              onChange={(event) => setQuestion(event.target.value)}
              aria-label="Question"
            />
            <label className="input-container__label" htmlFor="question">
              Question:
            </label>
          </div>
          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="answer"
              type="text"
              value={answer}
              onChange={(event) => setAnswer(event.target.value)}
              aria-label="Answer"
            />
            <label className="input-container__label" htmlFor="answer">
              Answer:
            </label>
          </div>
          <button className="button success-color" type="submit">
            Add Question
          </button>
        </form>
        <button onClick={closeModal} className="button danger-color">
          Cancel
        </button>
      </Modal>
    </>
  );
}
