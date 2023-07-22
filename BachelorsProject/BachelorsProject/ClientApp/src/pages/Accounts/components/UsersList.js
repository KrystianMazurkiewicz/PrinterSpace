import AnErrorOccured from "pages/AnErrorOccured/AnErrorOccured";
import React from "react";
import UserItem from "./UserItem";
import { uniqueId } from "../../../utils/customVanillaFunctions";

export default function UsersList({ users, onDelete }) {
  if (users.length === 0) return <h2>Normal user accounts are empty!</h2>;
  if (!users) return <AnErrorOccured />;

  return (
    <>
      <div className="page-wrapper">
        <header className="list-header">
          <h1>Users</h1>
        </header>
        <div className="list">
          {users.map((user) => {
            return (
              <UserItem user={user} onDelete={onDelete} key={uniqueId()} />
            );
          })}
        </div>
      </div>
    </>
  );
}
