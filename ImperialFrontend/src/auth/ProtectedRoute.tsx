import React from "react";
import { Navigate } from "react-router-dom";
import { useUser } from "./UserContext";

interface ProtectedRouteProps {
  children: React.ReactElement;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { user } = useUser();

  if (!user) {
    return <Navigate to="/" replace />;
  }

  return children;
};