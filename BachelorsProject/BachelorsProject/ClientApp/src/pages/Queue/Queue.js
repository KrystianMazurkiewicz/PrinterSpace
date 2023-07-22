import React, { useEffect, useState } from "react";
import ListNav from "components/ListNav";
import ListItemQueue from "./components/ListItemQueue";
import getPrintingQueue from "api/read/getPrintingQueue";
import cancelPrint from "api/delete/cancelPrint";
import { roleByToken } from "data/constants";
import Unauthorized from "pages/Unauthorized/Unauthorized";
import { ROLES } from "data/constants";

export default function Queue() {
  const [queueItems, setQueueItems] = useState([]);

  useEffect(() => {
    const abortController = new AbortController();
    getPrintingQueue(abortController)
      .then((data) => setQueueItems(data))
      .catch((error) => console.error(error));

    return () => {
      abortController.abort();
    };
  }, []);

  function onCancelPrint(print, username) {
    cancelPrint(print, username)
      .then(() =>
        getPrintingQueue()
          .then((data) => setQueueItems(data))
          .catch((error) => console.error(error))
      )
      .catch((error) => console.error(error));
  }

  if (roleByToken !== ROLES.admin) return <Unauthorized />;

  return (
    <>
      <ListNav pageName={"Queue"}>
        <ListItemQueue queue={queueItems} onCancelPrint={onCancelPrint} />
      </ListNav>
    </>
  );
}
