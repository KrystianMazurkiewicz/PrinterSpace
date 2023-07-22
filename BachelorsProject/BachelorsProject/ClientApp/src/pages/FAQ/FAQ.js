import React, { useEffect, useState } from "react";
import ListNav from "components/ListNav";
import { roleByToken } from "data/constants";
import deleteQAndA from "api/delete/deleteQAndA";
import getQAndA from "api/read/getQAndA";
import createQAndA from "api/create/createQAndA";
import editQAndA from "api/update/editQAndA";
import ListItemQAndA from "./components/ListItemQAndA";
import AddQAndA from "./components/AddQAndA";
import { ROLES } from "data/constants";

export default function FAQ() {
  const [QandAs, setQandAs] = useState([]);

  useEffect(() => {
    const abortController = new AbortController();
    updateQAndAList(abortController);

    return () => {
      abortController.abort();
    };
  }, []);

  function updateQAndAList(abortController) {
    getQAndA(abortController)
      .then((data) => setQandAs(data))
      .catch((error) => console.error(error));
  }

  function remove(id) {
    deleteQAndA(id).then(() => updateQAndAList());
  }

  function create(question, answer) {
    createQAndA(question, answer).then(() => updateQAndAList());
  }

  function edit(id, question, answer) {
    editQAndA(id, question, answer).then(() => updateQAndAList());
  }

  return (
    <ListNav pageName={"FAQ"}>
      <ListItemQAndA QandAs={QandAs} onRemove={remove} onEdit={edit} />
      {roleByToken === ROLES.admin && <AddQAndA onCreate={create} />}
    </ListNav>
  );
}
