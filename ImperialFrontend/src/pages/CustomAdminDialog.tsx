import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import IconButton from "@mui/material/IconButton";
import Tooltip from "@mui/material/Tooltip";
import CloseIcon from "@mui/icons-material/Close";

type AdminDialogProps = {
  open: boolean;
  onClose: () => void;
  title: string;
  children: React.ReactNode;
};

export const CustomAdminDialog = ({
  open,
  onClose,
  children,
}: AdminDialogProps) => (
  <Dialog
    open={open}
    onClose={(_e, reason) => {
      if (reason === "backdropClick" || reason === "escapeKeyDown") return;
      onClose();
    }}
    disableEscapeKeyDown
    maxWidth="xl"
    fullWidth
    slotProps={{
      paper: {
        sx: theme => ({
          borderRadius: 3,
          border: `1px solid ${theme.palette.divider}`,
          boxShadow: "0 20px 60px rgba(0,0,0,0.6)",
        }),
      },
    }}
  >
    <DialogContent sx={{ position: "relative", pt: 6 }}>
      <Tooltip title="Close window" arrow>
        <IconButton
          aria-label="close"
          onClick={onClose}
          sx={{
            position: "absolute",
            right: 8,
            top: 8,
            color: "text.secondary",
            "&:hover": {
              bgcolor: theme => theme.palette.primary.main + "22",

              color: "text.primary",
            },
          }}
        >
          <CloseIcon />
        </IconButton>
      </Tooltip>
      {children}
    </DialogContent>
  </Dialog>
);
