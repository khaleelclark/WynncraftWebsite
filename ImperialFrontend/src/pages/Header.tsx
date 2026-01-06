import SendIcon from "@mui/icons-material/Send";
import CabinRoundedIcon from "@mui/icons-material/CabinRounded";
import TerminalOutlinedIcon from "@mui/icons-material/TerminalOutlined";
import { ReactNode, useState } from "react";
import { useUser } from "../auth/UserContext";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import { ThemeProvider } from "@mui/material/styles";
import { BrowserRouter, Link } from "react-router-dom";
import theme from "../theme";

type HeaderButton = {
  text: string;
  link: string;
  icon?: ReactNode;
};

const headerButtons: HeaderButton[] = [
  {
    text: "Completed Raids",
    link: "/completed-raids-board",
    icon: <CabinRoundedIcon />,
  },
  {
    text: "Guild Members",
    link: "/",
    icon: <TerminalOutlinedIcon />,
  },
  {
    text: "Leaderboard",
    link: "/leaderboard",
    icon: <SendIcon />,
  },
];

export const Header = () => {
  const { user } = useUser();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  let el = (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Box
        component="nav"
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          px: 3,
          py: 2,
          bgcolor: "primary.main",
          color: "primary.contrastText",
          borderBottom: theme => `2px solid ${theme.palette.secondary.main}`,
          flexShrink: 0,
        }}
      >
        <Box
          sx={{
            display: "flex",
            gap: 2,
            alignItems: "center",
          }}
        >
          <Box
            component={Link}
            to="/"
            sx={{
              color: "inherit",
              textDecoration: "none",
              fontWeight: 700,
              "&:hover": { opacity: 0.9 },
            }}
          >
            Guild Members
          </Box>

          <Box
            component={Link}
            to="/leaderboard"
            sx={{
              color: "inherit",
              textDecoration: "none",
              fontWeight: 700,
              "&:hover": { opacity: 0.9 },
            }}
          >
            Leaderboard
          </Box>
          <Box
            component={Link}
            to="/completed-raids-board"
            sx={{
              color: "inherit",
              textDecoration: "none",
              fontWeight: 700,
              "&:hover": { opacity: 0.9 },
            }}
          >
            Completed Raids
          </Box>

          {user?.isAuthenticated && (
            <Box
              component={Link}
              to="/admin"
              sx={{
                color: "inherit",
                textDecoration: "none",
                fontWeight: 700,
                "&:hover": { opacity: 0.9 },
              }}
            >
              Admin Panel
            </Box>
          )}
        </Box>

        <Box sx={{ display: "flex", alignItems: "center", minWidth: 180 }}>
          {user && user.claims ? (
            <>
              <Button
                id="logout-button"
                aria-controls={open ? "logout-menu" : undefined}
                aria-haspopup="true"
                aria-expanded={open ? "true" : undefined}
                onClick={handleClick}
                variant="contained"
                color="secondary"
              >
                Welcome,{" "}
                {user.claims
                  ? user.claims.find((claim: any) => claim.type === "nickname")
                      .value
                  : ""}
              </Button>

              <Menu
                id="logout-menu"
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
                slotProps={{
                  paper: {
                    sx: {
                      bgcolor: "background.paper",
                      color: "text.primary",
                      border: theme => `1px solid ${theme.palette.divider}`,
                    },
                  },
                }}
              >
                <MenuItem
                  onClick={() => (window.location.href = "/api/auth/logout")}
                >
                  Logout
                </MenuItem>
                <MenuItem
                  onClick={() =>
                    (window.location.href = "/api/auth/logout-all")
                  }
                >
                  Logout from Authentik
                </MenuItem>
              </Menu>
            </>
          ) : (
            <Button
              variant="contained"
              color="secondary"
              onClick={() => {
                window.location.href = "/api/auth/login";
              }}
            >
              Admin Login
            </Button>
          )}
        </Box>
      </Box>
    </ThemeProvider>
  );
  return el;
};
