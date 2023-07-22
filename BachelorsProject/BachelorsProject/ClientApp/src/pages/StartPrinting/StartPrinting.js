import React, { useState } from "react";
import ListNav from "components/ListNav";
import ListItemQAndA from "./components/ListItemQAndA";
import UploadFile from "./components/UploadFile";
import uploadFile from "api/create/uploadFile";

export default function StartPrinting() {
  const [fileData, setFileData] = useState(null);
  const [file, setFile] = useState(null);

  function onUploadedFile(file, nameByToken) {
    setFile(file);
    if (window.confirm(`Are you sure you want to print ${file.name}?`)) {
      uploadFile(file, nameByToken)
        .then((data) => setFileData(data))
        .catch((error) => console.error(error));
    }
  }

  return (
    <>
      <ListNav pageName={"Start Printing"}>
        <ListItemQAndA fileData={fileData} file={file} />
        {!fileData && <UploadFile onUploadedFile={onUploadedFile} />}
      </ListNav>
    </>
  );
}
