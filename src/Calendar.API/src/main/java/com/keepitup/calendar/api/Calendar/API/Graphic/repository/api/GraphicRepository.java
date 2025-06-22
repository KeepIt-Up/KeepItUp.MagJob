package com.keepitup.calendar.api.Calendar.API.Graphic.repository.api;

import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface GraphicRepository extends JpaRepository<Graphic, BigInteger> {
    Optional<Graphic> findById(UUID uuid);
}
