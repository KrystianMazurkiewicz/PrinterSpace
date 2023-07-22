import React from "react";
import AddAdmin from "./AddAdmin";
import AdminItem from "./AdminItem";
import { uniqueId } from "../../../utils/customVanillaFunctions";

export default function AdminsList({ admins, onDelete, onCreateAdmin }) {
  if (admins && admins.length === 0)
    return <h2>An error occured. Could not find any Q&As.</h2>;

  return (
    <>
      <div className="page-wrapper">
        <header className="list-header">
          <h1>Admins</h1>
        </header>
        <div className="list">
          {admins.map((admin) => {
            return (
              <AdminItem admin={admin} onDelete={onDelete} key={uniqueId()} />
            );
          })}
          <AddAdmin onCreateAdmin={onCreateAdmin} />
        </div>
      </div>
    </>
  );
}
