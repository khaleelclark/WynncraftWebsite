// src/auth/UserContext.tsx
import React, { createContext, useContext, useEffect, useState } from "react";
import axios from "axios";

// Ensure cookies (imperial.auth) are sent
axios.defaults.withCredentials = true;

export type AuthMe = {
  isAuthenticated: boolean;
  nickname?: string;
};

type UserContextType = {
  user: AuthMe | null;
};

const UserContext = createContext<UserContextType>({
  user: null,
});

export const UserProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<AuthMe | null>(null);

  useEffect(() => {
    // Fetch current session; anonymous users return isAuthenticated=false.
    axios
      .get<AuthMe>("/api/auth/me")
      .then(res => {
        setUser(res.data);
      })
      .catch(() => {
        // Not logged in → normalize state
        setUser({ isAuthenticated: false });
      });
  }, []);

  return (
    <UserContext.Provider value={{ user }}>{children}</UserContext.Provider>
  );
};

export const useUser = () => useContext(UserContext);
