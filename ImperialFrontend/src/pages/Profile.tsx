import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

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
}

const Profile: React.FC = () => {
  const { id } = useParams();
  const [profile, setProfile] = useState<ProfileData | null>(null);

  useEffect(() => {
    fetch(`/api/guildmembers/${id}`)
      .then((res) => res.json())
      .then((data) => {
        if (!data) return setProfile(null);

        // Map snake_case to camelCase
        const keyMap: Record<string, string> = {
          main_username: "mainUsername",
          minecraft_username: "minecraftUsername",
          rank_name: "rankName",
          join_date: "joinDate",
          wynncraft_rank: "wynncraftRank",
          hours_played: "hoursPlayed",
          wars_completed: "warsCompleted",
          weeklies_completed: "weekliesCompleted",
          games: "games",
          medals: "medals",
          raids_completed: "raidsCompleted",
          uuid: "uuid",
        };
        const mapped: any = {};
        Object.keys(data).forEach((key) => {
          mapped[keyMap[key] || key] = data[key];
        });
        setProfile(mapped);
      });
  }, [id]);

  if (!profile) return <div>Loading...</div>;

  return (
    <div style={{ padding: 24 }}>
      <h2>Profile: {profile.mainUsername}</h2>
      <img
        src={`https://crafatar.com/avatars/${profile.uuid}?size=64&overlay`}
        alt="Skin Face"
        width={64}
        height={64}
        style={{
          borderRadius: 8,
          border: "1px solid #ccc",
          display: "inline-block",
        }}
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
        <li>Raids Completed: {profile.raidsCompleted ?? 0}</li>
      </ul>
    </div>
  );
};

export default Profile;
