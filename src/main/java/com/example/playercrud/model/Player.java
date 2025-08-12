package com.example.playercrud.model;

import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;

@Entity
public class Player {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String displayName;
    private int daysInGuild;
    private int raids;
    private int wars;
    private int xp;
    private boolean hasWarBuild;
    private double weeklyPlaytimeHours;
    private int level105Classes;
    private int weeklyMissions;
    private boolean topContributor;

    // Getters and Setters

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getDisplayName() {
        return displayName;
    }

    public void setDisplayName(String displayName) {
        this.displayName = displayName;
    }

    public int getDaysInGuild() {
        return daysInGuild;
    }

    public void setDaysInGuild(int daysInGuild) {
        this.daysInGuild = daysInGuild;
    }

    public int getRaids() {
        return raids;
    }

    public void setRaids(int raids) {
        this.raids = raids;
    }

    public int getWars() {
        return wars;
    }

    public void setWars(int wars) {
        this.wars = wars;
    }

    public int getXp() {
        return xp;
    }

    public void setXp(int xp) {
        this.xp = xp;
    }

    public boolean isHasWarBuild() {
        return hasWarBuild;
    }

    public void setHasWarBuild(boolean hasWarBuild) {
        this.hasWarBuild = hasWarBuild;
    }

    public double getWeeklyPlaytimeHours() {
        return weeklyPlaytimeHours;
    }

    public void setWeeklyPlaytimeHours(double weeklyPlaytimeHours) {
        this.weeklyPlaytimeHours = weeklyPlaytimeHours;
    }

    public int getLevel105Classes() {
        return level105Classes;
    }

    public void setLevel105Classes(int level105Classes) {
        this.level105Classes = level105Classes;
    }

    public int getWeeklyMissions() {
        return weeklyMissions;
    }

    public void setWeeklyMissions(int weeklyMissions) {
        this.weeklyMissions = weeklyMissions;
    }

    public boolean isTopContributor() {
        return topContributor;
    }

    public void setTopContributor(boolean topContributor) {
        this.topContributor = topContributor;
    }

    public void getTopContributor(boolean topContributor) {
        this.topContributor = topContributor;
    }
}