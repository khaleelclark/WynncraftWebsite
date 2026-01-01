import CircularProgress from "@mui/material/CircularProgress";
import Box from "@mui/material/Box";
import axios from "axios";
import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";

import Avatar from "@mui/material/Avatar";
import Stack from "@mui/material/Stack";
import Chip from "@mui/material/Chip";
import Divider from "@mui/material/Divider";
import dayjs from "dayjs";

interface ProfileData {
  mainUsername: string;
  minecraftUsername: string;
  rankName?: string;
  joinDate?: string;
  wynncraftRank?: string;
  hoursPlayed?: number;
  warsCompleted?: number;
  weekliesCompleted?: number;
  games?: string[];
  medals?: string[];
  raidsCompleted?: number;
  uuid?: string;
  lastSynced?: string;
}

const Profile: React.FC = () => {
  const { id } = useParams();
  const [profile, setProfile] = useState<ProfileData | null>(null);

  useEffect(() => {
    axios.get(`/api/guildmembers/${id}`).then((res) => {
      if (!res.data) {
        return setProfile(null);
      } else {
        return setProfile(res.data);
      }
    });
  }, [id]);

  if (!profile) {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "80vh",
          width: "100%",
        }}
      >
        <CircularProgress />
      </Box>
    );
  } else
    return (
      <Box
        sx={{
          p: 3,
          display: "flex",
          justifyContent: "center",
        }}
      >
        <Paper
          sx={{
            p: 3,
            maxWidth: 700,
            bgcolor: "#220c0e",
            color: "#efdddb",
            borderRadius: 2,
            boxShadow: 4,
          }}
        >
          {/* Header: avatar + name */}
          <Grid container spacing={2} alignItems="center">
            <Grid>
              <Avatar
                variant="rounded"
                src={`https://mc-heads.net/avatar/${profile.uuid}/100/`}
                alt="Skin Face"
                sx={{
                  width: 72,
                  height: 72,
                  borderRadius: 2,
                  border: "2px solid #efdddb",
                }}
              />
            </Grid>
            <Grid>
              <Typography variant="h4" sx={{ fontWeight: 600 }}>
                {profile.mainUsername}
              </Typography>
              <Typography variant="subtitle1" sx={{ opacity: 0.8 }}>
                {profile.minecraftUsername}
              </Typography>
            </Grid>
          </Grid>

          <Divider sx={{ my: 3, borderColor: "#7A1C69" }} />

          {/* Basic stats in a grid */}
          <Grid container spacing={2}>
            <Grid>
              <Typography variant="caption" sx={{ opacity: 0.7 }}>
                Rank
              </Typography>
              <Typography variant="body1">{profile.rankName}</Typography>
            </Grid>

            <Grid>
              <Typography variant="caption" sx={{ opacity: 0.7 }}>
                Join Date
              </Typography>
              <Typography variant="body1">{profile.joinDate}</Typography>
            </Grid>

            <Grid>
              <Typography variant="caption" sx={{ opacity: 0.7 }}>
                Wynncraft Rank
              </Typography>
              <Typography variant="body1">{profile.wynncraftRank}</Typography>
            </Grid>

            <Grid>
              <Typography variant="caption" sx={{ opacity: 0.7 }}>
                Raids Completed
              </Typography>
              <Typography variant="body1">{profile.raidsCompleted}</Typography>
            </Grid>

            <Grid>
              <Typography variant="caption" sx={{ opacity: 0.7 }}>
                Hours Played
              </Typography>
              <Typography variant="body1">{profile.hoursPlayed}</Typography>
            </Grid>

            <Grid>
              <Typography variant="caption" sx={{ opacity: 0.7 }}>
                Wars Completed
              </Typography>
              <Typography variant="body1">{profile.warsCompleted}</Typography>
            </Grid>

            <Grid>
              <Typography variant="caption" sx={{ opacity: 0.7 }}>
                Weeklies Completed
              </Typography>
              <Typography variant="body1">
                {profile.weekliesCompleted}
              </Typography>
            </Grid>
          </Grid>

          <Grid>
            <Typography variant="caption" sx={{ opacity: 0.7 }}>
              Last Updated
            </Typography>
            <Typography variant="body1">
              {dayjs(profile.lastSynced).format("YYYY-MM-DD hh:mm A")}
            </Typography>
          </Grid>

          {/* Games */}
          <Box sx={{ mt: 3 }}>
            <Typography
              variant="caption"
              sx={{ opacity: 0.7, display: "block" }}
            >
              Games
            </Typography>
            {profile.games && profile.games.length > 0 ? (
              <Stack direction="row" flexWrap="wrap" gap={1} mt={1}>
                {profile.games.map((game) => (
                  <Chip
                    key={game}
                    label={game}
                    size="small"
                    sx={{ bgcolor: "#3C002F", color: "#efdddb" }}
                  />
                ))}
              </Stack>
            ) : (
              <Typography variant="body2" sx={{ opacity: 0.8 }}>
                None listed
              </Typography>
            )}
          </Box>

          {/* Medals */}
          <Box sx={{ mt: 2 }}>
            <Typography
              variant="caption"
              sx={{ opacity: 0.7, display: "block" }}
            >
              Medals
            </Typography>
            {profile.medals && profile.medals.length > 0 ? (
              <Stack direction="row" flexWrap="wrap" gap={1} mt={1}>
                {profile.medals.map((medal) => (
                  <Chip
                    key={medal}
                    label={medal}
                    size="small"
                    sx={{ bgcolor: "#511220", color: "#efdddb" }}
                  />
                ))}
              </Stack>
            ) : (
              <Typography variant="body2" sx={{ opacity: 0.8 }}>
                None earned yet
              </Typography>
            )}
          </Box>
        </Paper>
      </Box>
    );
};

export default Profile;
