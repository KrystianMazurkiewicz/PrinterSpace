import React, { useEffect, useState } from "react";
import { roleByToken } from "data/constants";
import Unauthorized from "pages/Unauthorized/Unauthorized";
import AccountsWrapper from "./components/AccountsWrapper";
import AdminsList from "./components/AdminsList";
import UsersList from "./components/UsersList";
import getAllAdmins from "api/read/getAllAdmins";
import getAllUsers from "api/read/getAllUsers";
import deleteAdmin from "api/delete/deleteAdmin";
import AnErrorOccured from "pages/AnErrorOccured/AnErrorOccured";
import createAdmin from "api/create/createAdmin";
import deleteUser from "api/delete/deleteUser";
import { ROLES } from "data/constants";

export default function Accounts() {
  const [adminAccounts, setAdminAccounts] = useState([]);
  const [userAccounts, setUserAccounts] = useState([]);

  useEffect(() => {
    const abortController = new AbortController();
    updateAdminAccounts(abortController);

    return () => {
      abortController.abort();
    };
  }, []);

  useEffect(() => {
    const abortController = new AbortController();
    updateUserAccounts(abortController);

    return () => {
      abortController.abort();
    };
  }, []);

  function updateAdminAccounts(abortController) {
    getAllAdmins(abortController).then((data) => setAdminAccounts(data));
  }

  function updateUserAccounts(abortController) {
    getAllUsers(abortController).then((data) => setUserAccounts(data));
  }

  function onDeleteAdmin(username) {
    const abortController = new AbortController();
    deleteAdmin(username).then(() => updateAdminAccounts(abortController));
  }

  function onDeleteUser(username) {
    const abortController = new AbortController();
    deleteUser(username).then(() => updateUserAccounts(abortController));
  }

  function onCreateAdmin(username) {
    const abortController = new AbortController();
    createAdmin(username).then(() => updateAdminAccounts(abortController));
  }

  if (roleByToken !== ROLES.admin) return <Unauthorized />;
  if (!adminAccounts || !userAccounts) return <AnErrorOccured />;

  return (
    <>
      <AccountsWrapper>
        <AdminsList
          admins={adminAccounts}
          onDelete={onDeleteAdmin}
          onCreateAdmin={onCreateAdmin}
        />
        <UsersList users={userAccounts} onDelete={onDeleteUser} />
      </AccountsWrapper>
    </>
  );
}
