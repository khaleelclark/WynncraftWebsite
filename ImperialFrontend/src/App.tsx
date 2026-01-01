import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import { ThemeProvider, CssBaseline } from "@mui/material";
import theme from "./theme";
import AdminPanel from "./pages/AdminPanel";
import GuildMemberList from "./pages/GuildMemberList";
import Profile from "./pages/Profile";
import Leaderboard from "./pages/Leaderboard";
import AddEvent from "./pages/AddEvent";
import EventTablePage from "./pages/EventTablePage";

const App: React.FC = () => (
  <ThemeProvider theme={theme}>
    <CssBaseline />
    <BrowserRouter>
      <nav
        style={{
          padding: 16,
          borderBottom: "2px solid #bc511c",
          background: "#82172e",
          color: "#efdddb",
          marginBottom: 24,
        }}
      >
        <Link
          to="/"
          style={{
            color: "#efdddb",
            marginRight: 16,
            textDecoration: "none",
            fontWeight: 600,
          }}
        >
          Guild Members
        </Link>
        <Link
          to="/leaderboard"
          style={{
            color: "#efdddb",
            textDecoration: "none",
            fontWeight: 600,
            marginRight: 16,
          }}
        >
          Leaderboard
        </Link>
        <Link
          to="/admin"
          style={{
            color: "#efdddb",
            textDecoration: "none",
            fontWeight: 600,
            marginRight: 16,
          }}
        >
          Admin Panel
        </Link>
      </nav>
      <Routes>
        <Route path="/" element={<GuildMemberList />} />
        <Route path="/profile/:id" element={<Profile />} />
        <Route path="/leaderboard" element={<Leaderboard />} />
        <Route path="/add-event" element={<AddEvent />} />
        <Route path="/admin" element={<AdminPanel />} />
        <Route path="/events" element={<EventTablePage />} />
      </Routes>
    </BrowserRouter>
  </ThemeProvider>
);

export default App;
