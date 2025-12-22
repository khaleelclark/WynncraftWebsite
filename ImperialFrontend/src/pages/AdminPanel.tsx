import Button from "@mui/material/Button";
import { useNavigate } from "react-router-dom";

const AdminPanel: React.FC = () => {
  const navigate = useNavigate();
  return (
    <div style={{ padding: 32 }}>
      <h2>Admin Panel</h2>
      <Button
        variant="contained"
        color="primary"
        onClick={() => navigate("/events")}
      >
        Go to Event Table
      </Button>
      {/* More admin features will go here */}
    </div>
  );
};

export default AdminPanel;
