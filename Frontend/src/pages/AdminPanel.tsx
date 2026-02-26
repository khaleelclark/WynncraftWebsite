import { useState } from "react";
import { CustomForm } from "./CustomForm";
import { CustomTextField } from "./CustomTextField";
import { CustomDropdown } from "./CustomDropdown";
import { CustomDateOnlySelector } from "./CustomDateOnlySelector";
import { GenericAdminPage } from "./GenericAdminPage";
import { CustomTimeAndDateSelector } from "./CustomTimeAndDateSelector";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Paper from "@mui/material/Paper";
import Grid from "@mui/material/Grid";
import GroupIcon from "@mui/icons-material/Group";
import EventIcon from "@mui/icons-material/Event";
import SportsEsportsIcon from "@mui/icons-material/SportsEsports";
import MilitaryTechIcon from "@mui/icons-material/MilitaryTech";
import WorkspacePremiumIcon from "@mui/icons-material/WorkspacePremium";
import SecurityIcon from "@mui/icons-material/Security";
import { CustomAdminTile } from "./CustomAdminTile";
import { CustomAdminDialog } from "./CustomAdminDialog";
import VerifiedUserIcon from "@mui/icons-material/VerifiedUser";
import { Header } from "./Header";

const AdminPanel: React.FC = () => {
  // Dialog state per admin surface.
  const [guildMembersOpen, setGuildMembersOpen] = useState(false);
  const [eventsOpen, setEventsOpen] = useState(false);
  const [gamesOpen, setGamesOpen] = useState(false);
  const [medalsOpen, setMedalsOpen] = useState(false);
  const [ranksOpen, setRanksOpen] = useState(false);
  const [raidsOpen, setRaidsOpen] = useState(false);
  const [raidsCompletedOpen, setRaidsCompletedOpen] = useState(false);

  // Per-entity form definitions are reused by GenericAdminPage.
  const createGuildMemberForm = (
    <CustomForm title="Guild Member Management" apiEndpoint="/api/guildmembers">
      <CustomTextField id="discordTag" label="Discord Tag" minLength={1} />
      <CustomTextField id="name" label="Main Username" minLength={1} />
      <CustomTextField
        id="uuid"
        label="Minecraft UUID"
        minLength={32}
        format="UUID"
      />
      <CustomDropdown id="rank" label="Rank" apiEndpoint="/api/ranks" />
      <CustomDropdown
        id="medals"
        label="Medals"
        apiEndpoint="/api/medals"
        multiple={true}
        required={false}
      />
      <CustomDropdown
        id="games"
        label="Games"
        apiEndpoint="/api/games"
        multiple={true}
      />
      <CustomDateOnlySelector id="joinDate" label="Join Date" />
    </CustomForm>
  );

  const createEventForm = (
    <CustomForm title="Event Management" apiEndpoint="/api/events">
      <CustomTextField id="name" label="Event Name" minLength={4} />
      <CustomTimeAndDateSelector id="eventStart" label="Start Date & Time" />
      <CustomTimeAndDateSelector id="eventEnd" label="End Date & Time" />
    </CustomForm>
  );

  const createGameForm = (
    <CustomForm title="Game Management" apiEndpoint="/api/games">
      <CustomTextField id="name" label="Game Name" minLength={2} />
    </CustomForm>
  );

  const createMedalForm = (
    <CustomForm title="Medal Management" apiEndpoint="/api/medals">
      <CustomTextField id="name" label="Medal Name" minLength={2} />
    </CustomForm>
  );

  const createRankForm = (
    <CustomForm title="Rank Management" apiEndpoint="/api/ranks">
      <CustomTextField id="name" label="Rank Name" minLength={2} />
    </CustomForm>
  );

  const createRaidForm = (
    <CustomForm title="Raid Management" apiEndpoint="/api/raids">
      <CustomTextField id="id" label="Raid Id" minLength={1} type="number" />
      <CustomTextField id="name" label="Raid Name" minLength={3} />
      <CustomTextField
        id="seasonRating"
        label="Season Rating"
        minLength={1}
        type="number"
      />
    </CustomForm>
  );

  const createRaidCompletedForm = (
    <CustomForm
      title="Raids Completed Management"
      apiEndpoint="/api/raidscompleted"
    >
      <CustomDropdown id="raid" label="Raid Id" apiEndpoint="/api/raids" />
      <CustomTimeAndDateSelector
        id="completedDate"
        label="Completed Time and Date"
      />
      <CustomDropdown
        id="guildMembers"
        label="Players"
        apiEndpoint="/api/guildmembers"
        multiple={true}
      />
    </CustomForm>
  );

  return (
    <>
      <Header />

      <Box
        sx={{
          p: 3,
          display: "flex",
          justifyContent: "center",
          bgcolor: "background.default",
          color: "text.primary",
        }}
      >
        <Box sx={{ width: "100%", maxWidth: 1200 }}>
          <Paper
            elevation={0}
            sx={{
              p: 4,
              maxWidth: "100%",
              bgcolor: "background.paper",
              color: "text.primary",
              borderRadius: 3,
              border: theme => `1px solid ${theme.palette.divider}`,
              boxShadow: "0 16px 40px rgba(0,0,0,0.55)",
            }}
          >
            <Typography
              variant="h3"
              textAlign="center"
              gutterBottom
              sx={{
                fontWeight: 800,
                color: "text.primary",
              }}
            >
              Novus Guild Admin Panel
            </Typography>
            <Typography
              variant="subtitle1"
              textAlign="center"
              sx={{
                color: "text.secondary",
                mb: 4,
              }}
            >
              Manage all Novus guild information in one place.
            </Typography>

            {/* --- TILE GRID --- */}
            <Grid
              container
              spacing={3}
              justifyContent="center"
              alignItems="stretch"
              sx={{ mt: 1 }}
            >
              <CustomAdminTile
                title="Guild Members"
                description="View, edit, and manage all guild members and their details."
                icon={<GroupIcon />}
                onOpen={() => setGuildMembersOpen(true)}
              />

              <CustomAdminTile
                title="Events"
                description="Configure upcoming raid events, wars, and guild activities."
                icon={<EventIcon />}
                onOpen={() => setEventsOpen(true)}
              />

              <CustomAdminTile
                title="Medals"
                description="Create and assign medals to recognize achievements."
                icon={<MilitaryTechIcon />}
                onOpen={() => setMedalsOpen(true)}
              />

              <CustomAdminTile
                title="Ranks"
                description="Create, edit and delete guild ranks."
                icon={<WorkspacePremiumIcon />}
                onOpen={() => setRanksOpen(true)}
              />

              <CustomAdminTile
                title="Games"
                description="Create, edit and delete supported games."
                icon={<SportsEsportsIcon />}
                onOpen={() => setGamesOpen(true)}
              />

              <CustomAdminTile
                title="Raids"
                description="Create, edit and delete guild raids."
                icon={<SecurityIcon />}
                onOpen={() => setRaidsOpen(true)}
              />
              <CustomAdminTile
                title="Raids Completed"
                description="Create, edit and delete completed guild raids.(Rare use cases only)"
                icon={<VerifiedUserIcon />}
                onOpen={() => setRaidsCompletedOpen(true)}
              />
            </Grid>
          </Paper>

          {/* ----- Dialogs: open the GenericAdminPage with the right endpoints ----- */}
          <CustomAdminDialog
            open={guildMembersOpen}
            onClose={() => setGuildMembersOpen(false)}
            title="Guild Members"
          >
            <GenericAdminPage
              label="Guild Members"
              apiGetEndpoint="/api/guildmembers/admin"
              apiDeleteEndpoint="/api/guildmembers"
              createForm={createGuildMemberForm}
            />
          </CustomAdminDialog>
          <CustomAdminDialog
            open={eventsOpen}
            onClose={() => setEventsOpen(false)}
            title="Events"
          >
            <GenericAdminPage
              label="Events"
              apiGetEndpoint="/api/events"
              apiDeleteEndpoint="/api/events"
              createForm={createEventForm}
            />
          </CustomAdminDialog>

          <CustomAdminDialog
            open={gamesOpen}
            onClose={() => setGamesOpen(false)}
            title="Games"
          >
            <GenericAdminPage
              label="Games"
              apiGetEndpoint="/api/games"
              apiDeleteEndpoint="/api/games"
              createForm={createGameForm}
            />
          </CustomAdminDialog>

          <CustomAdminDialog
            open={medalsOpen}
            onClose={() => setMedalsOpen(false)}
            title="Medals"
          >
            <GenericAdminPage
              label="Medals"
              apiGetEndpoint="/api/medals"
              apiDeleteEndpoint="/api/medals"
              createForm={createMedalForm}
            />
          </CustomAdminDialog>

          <CustomAdminDialog
            open={ranksOpen}
            onClose={() => setRanksOpen(false)}
            title="Ranks"
          >
            <GenericAdminPage
              label="Ranks"
              apiGetEndpoint="/api/ranks"
              apiDeleteEndpoint="/api/ranks"
              createForm={createRankForm}
            />
          </CustomAdminDialog>

          <CustomAdminDialog
            open={raidsOpen}
            onClose={() => setRaidsOpen(false)}
            title="Raids"
          >
            <GenericAdminPage
              label="Raids"
              apiGetEndpoint="/api/raids"
              apiDeleteEndpoint="/api/raids"
              createForm={createRaidForm}
            />
          </CustomAdminDialog>

          <CustomAdminDialog
            open={raidsCompletedOpen}
            onClose={() => setRaidsCompletedOpen(false)}
            title="Raids Completed"
          >
            <GenericAdminPage
              label="Raids Completed"
              apiGetEndpoint="/api/raidscompleted"
              apiDeleteEndpoint="/api/raidscompleted"
              createForm={createRaidCompletedForm}
            />
          </CustomAdminDialog>
        </Box>
      </Box>
    </>
  );
};

export default AdminPanel;
