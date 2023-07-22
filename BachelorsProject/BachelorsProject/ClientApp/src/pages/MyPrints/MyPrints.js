import React, { useEffect, useState } from "react";
import ListNav from "components/ListNav";
import ListItemHistory from "./components/ListItemHistory";
import getPrintingHistoryForUser from "api/read/getPrintingHistoryForUser";
import getPrintingQueueForUser from "api/read/getPrintingQueueForUser";
import cancelUserPrint from "api/delete/cancelUserPrint";

export default function MyPrints(e) {
  const [printingHistory, setPrintingHistory] = useState(null);
  const [queue, setQueue] = useState(null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [combinedArray, setCombinedArray] = useState([]);

  useEffect(() => {
    if (printingHistory && queue) {
      setCombinedArray([...printingHistory, ...queue]);
    } else if (!printingHistory && queue) {
      setCombinedArray([...queue]);
    } else if (printingHistory && !queue) {
      setCombinedArray([...printingHistory]);
    } else {
      setCombinedArray([]);
    }
  }, [printingHistory, queue]);

  useEffect(() => {
    const abortController = new AbortController();

    setIsLoading(true);

    Promise.all([
      getPrintingHistoryForUser(abortController),
      getPrintingQueueForUser(abortController),
    ])
      .then(([historyData, queueData]) => {
        setPrintingHistory(historyData);
        setQueue(queueData);

        setIsLoading(false);
      })
      .catch((error) => {
        console.error(error);

        setError(error);
        setIsLoading(false);
      });

    return () => {
      abortController.abort();
    };
  }, []);

  function updateList(abortController) {
    getPrintingQueueForUser(abortController)
      .then((data) => setQueue(data))
      .catch((error) => console.error(error));

    getPrintingHistoryForUser(abortController)
      .then((data) => setPrintingHistory(data))
      .catch((error) => console.error(error));
  }

  function onCancelPrint(print, username) {
    cancelUserPrint(print, username)
      .then(() => updateList())
      .catch((error) => console.error(error));
  }

  const alternativeContent = () => {
    if (isLoading) {
      return <h2>Loading...</h2>;
    } else if (error) {
      return <h2>An error occurred: {error.message}</h2>;
    } else if (!combinedArray || combinedArray.length === 0) {
      return <h2>Your history is empty</h2>;
    }
  };

  return (
    <ListNav pageName={"My Prints"}>
      <ListItemHistory
        printingHistory={combinedArray}
        alternativeContent={alternativeContent()}
        onCancelPrint={onCancelPrint}
      />
    </ListNav>
  );
}
