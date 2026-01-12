import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ThemeProvider, CssBaseline } from "@mui/material";
import theme from "./theme";
import AdminPanel from "./pages/AdminPanel";
import GuildMemberList from "./pages/GuildMemberList";
import Profile from "./pages/Profile";
import Leaderboard from "./pages/Leaderboard";
import GuildRaidsBoard from "./pages/GuildRaidsBoard";
import { RequireAuth } from "./auth/RequireAuth";

export const App = () => {
  const el = (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Routes>
          <Route path="/completed-raids-board" element={<GuildRaidsBoard />} />
          <Route path="/" element={<GuildMemberList />} />
          <Route path="/profile/:id" element={<Profile />} />
          <Route path="/leaderboard" element={<Leaderboard />} />
          <Route
            path="/admin"
            element={
              <RequireAuth>
                <AdminPanel />
              </RequireAuth>
            }
          />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );

  return el;
};
