import React from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useUser } from "./UserContext";

export function RequireAuth({ children }: { children: React.ReactElement }) {
  const { user } = useUser();
  const location = useLocation();

  // Still loading (we haven't finished calling /api/auth/me yet)
  if (user === null) return null; // or return <Spinner />

  // Finished loading, but not authenticated
  if (!user.isAuthenticated) {
    // Preserve the attempted path for post-login navigation.
    return <Navigate to="/" replace state={{ from: location.pathname }} />;
  }

  return children;
}
