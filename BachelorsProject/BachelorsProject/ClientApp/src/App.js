import React, { useEffect, useState } from "react";
import { Route, Routes } from "react-router-dom";
import NotFound from "./pages/NotFound/NotFound";
import MyPrints from "./pages/MyPrints/MyPrints";
import FAQ from "./pages/FAQ/FAQ";
import Queue from "./pages/Queue/Queue";
import Printers from "./pages/Printers/Printers";
import Login from "./pages/Login/Login";
import Menu from "./pages/Menu/Menu";
import Accounts from "pages/Accounts/Accounts";
import StartPrinting from "pages/StartPrinting/StartPrinting";
import Register from "pages/Register/Register";
import { roleByToken } from "data/constants";

export default function App() {
  if (!roleByToken)
    return (
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/Register" element={<Register />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    );

  return (
    <Routes>
      <Route path="/" element={<Menu />} />
      <Route path="/My_Prints" element={<MyPrints />} />
      <Route path="/FAQ" element={<FAQ />} />
      <Route path="/Accounts" element={<Accounts />} />
      <Route path="/Printers" element={<Printers />} />
      <Route path="/Queue" element={<Queue />} />
      <Route path="/Start_Printing" element={<StartPrinting />} />
      <Route path="/Register" element={<Register />} />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
}
