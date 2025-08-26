import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

interface ProfileData {
  mainUsername: string;
  minecraftUsername: string;
  rankName?: string;
  playerSkin?: string;
  joinDate?: string;
  wynncraftRank?: string;
  hoursPlayed?: number;
  warsCompleted?: number;
  weekliesCompleted?: number;
  games?: string[];
  medals?: string[];
  raidsCompleted?: number;
}

const Profile: React.FC = () => {
  const { id } = useParams();
  const [profile, setProfile] = useState<ProfileData | null>(null);

  useEffect(() => {
    fetch(`/odata/GuildMembers/Profile/${id}`)
      .then((res) => res.json())
      .then(setProfile);
  }, [id]);

  if (!profile) return <div>Loading...</div>;

  return (
    <div style={{ padding: 24 }}>
      <h2>Profile: {profile.mainUsername}</h2>
      <img
        src={profile.playerSkin}
        alt="Skin"
        style={{ width: 64, height: 64 }}
      />
      <ul>
        <li>Minecraft Username: {profile.minecraftUsername}</li>
        <li>Rank: {profile.rankName}</li>
        <li>Join Date: {profile.joinDate?.slice(0, 10)}</li>
        <li>Wynncraft Rank: {profile.wynncraftRank}</li>
        <li>Hours Played: {profile.hoursPlayed}</li>
        <li>Wars Completed: {profile.warsCompleted}</li>
        <li>Weeklies Completed: {profile.weekliesCompleted}</li>
        <li>Games: {profile.games?.join(", ")}</li>
        <li>Medals: {profile.medals?.join(", ")}</li>
        <li>Raids Completed: {profile.raidsCompleted}</li>
      </ul>
    </div>
  );
};

export default Profile;
