import * as React from "react";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import { SxProps, Theme } from "@mui/material/styles";

type AdminTileProps = {
  title: string;
  description: string;
  icon: React.ReactNode;
  onOpen: () => void;
  buttonText?: string;
};

export const CustomAdminTile = ({
  title,
  description,
  icon,
  onOpen,
  buttonText = "Open",
}: AdminTileProps) => {
  return (
    <Grid display="flex">
      <Box
        sx={{
          width: "100%",
          display: "flex",
          justifyContent: "center",
        }}
      >
        <Box
          sx={{
            width: "clamp(260px, 90vw, 360px)",
            height: { xs: 200, sm: 220, md: 240 },
            display: "flex",
          }}
        >
          <Paper
            onClick={onOpen}
            sx={{
              p: 3,
              flexGrow: 1,
              borderRadius: 3,

              cursor: "pointer",
              display: "flex",
              flexDirection: "column",
              transition: "transform 0.15s ease, box-shadow 0.15s ease",

              color: "text.primary",
              border: theme => `1px solid ${theme.palette.divider}`,
              boxShadow: 6,

              "&:hover": {
                transform: "translateY(-4px)",
                boxShadow: 10,
                borderColor: theme => theme.palette.primary.main,
              },
            }}
          >
            {/* Header */}
            <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
              <Box
                sx={{
                  mr: 2,
                  p: 1.2,
                  borderRadius: "999px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  bgcolor: theme => `${theme.palette.background.default}66`,
                  border: theme => `1px solid ${theme.palette.primary.main}55`,
                  color: "text.secondary",
                }}
              >
                {icon}
              </Box>
              <Typography variant="h5" sx={{ fontWeight: 600 }}>
                {title}
              </Typography>
            </Box>

            {/* Description */}
            <Typography variant="body2" sx={{ opacity: 0.85, flexGrow: 1 }}>
              {description}
            </Typography>

            {/* Button pinned to bottom */}
            <Button
              variant="contained"
              color="primary"
              onClick={e => {
                e.stopPropagation();
                onOpen();
              }}
              sx={{
                mt: 2,
                alignSelf: "flex-start",
                boxShadow: theme => `0 0 14px ${theme.palette.primary.main}33`,
                "&:hover": {
                  boxShadow: theme =>
                    `0 0 18px ${theme.palette.primary.main}66`,
                },
              }}
            >
              {buttonText}
            </Button>
          </Paper>
        </Box>
      </Box>
    </Grid>
  );
};
