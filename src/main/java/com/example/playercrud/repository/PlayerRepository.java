package com.example.playercrud.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import com.example.playercrud.model.Player;

@Repository
public interface PlayerRepository extends JpaRepository<Player, Long> {
}