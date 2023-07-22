import React, { useRef } from "react";
import PlusInsideCircleIcon from "components/icons/PlusInsideCircleIcon";
import { nameByToken } from "data/constants";

export default function UploadFile({ onUploadedFile }) {
  const fileInput = useRef();

  const handleUploadedFile = async (event) => {
    event.preventDefault();
    const username = nameByToken === "" ? null : nameByToken;
    onUploadedFile(fileInput.current.files[0], nameByToken);
  };

  return (
    <>
      <label
        htmlFor="upload-gcode-file-input"
        className="list-item upload-gcode-file-label cursor-default"
        onChange={handleUploadedFile}
      >
        <div className="item-container">
          <input
            type="file"
            accept=".gcode"
            ref={fileInput}
            id="upload-gcode-file-input"
          />
          <PlusInsideCircleIcon />
          <p className="file-name">Upload your .gcode file</p>
        </div>
      </label>
    </>
  );
}
