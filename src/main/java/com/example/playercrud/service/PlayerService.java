package com.example.playercrud.service;

import com.example.playercrud.model.Player;
import com.example.playercrud.repository.PlayerRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class PlayerService {

    @Autowired
    private PlayerRepository playerRepository;

    public Player createPlayer(Player player) {
        return playerRepository.save(player);
    }

    public Optional<Player> getPlayerById(Long id) {
        return playerRepository.findById(id);
    }

    public List<Player> getAllPlayers() {
        return playerRepository.findAll();
    }

    public Player updatePlayer(Long id, Player playerDetails) {
        Player player = playerRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Player not found with id " + id));
        player.setDisplayName(playerDetails.getDisplayName());
        player.setDaysInGuild(playerDetails.getDaysInGuild());
        player.setRaids(playerDetails.getRaids());
        player.setWars(playerDetails.getWars());
        player.setXp(playerDetails.getXp());
       // player.setHasWarBuild(playerDetails.getHasWarBuild());
        player.setWeeklyPlaytimeHours(playerDetails.getWeeklyPlaytimeHours());
        player.setLevel105Classes(playerDetails.getLevel105Classes());
        player.setWeeklyMissions(playerDetails.getWeeklyMissions());
        //player.setTopContributor(playerDetails.getTopContributor());
        return playerRepository.save(player);
    }

    public boolean deletePlayer(Long id) {
        if (playerRepository.existsById(id)) {
            playerRepository.deleteById(id);
            return true;
        }
        return false;
    }
}