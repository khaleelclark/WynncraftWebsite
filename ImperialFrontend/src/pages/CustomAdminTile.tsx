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
              bgcolor: "#82172e",
              cursor: "pointer",
              display: "flex",
              flexDirection: "column",
              transition: "transform 0.15s ease, box-shadow 0.15s ease",
              boxShadow: 3,
              "&:hover": {
                transform: "translateY(-4px)",
                boxShadow: 8,
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
                  bgcolor: "rgba(0,0,0,0.25)",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
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
              onClick={e => {
                e.stopPropagation();
                onOpen();
              }}
              sx={{
                mt: 2,
                alignSelf: "flex-start",
                bgcolor: "#efdddb",
                color: "#501117ff",
                "&:hover": { bgcolor: "#f6e8e6" },
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
