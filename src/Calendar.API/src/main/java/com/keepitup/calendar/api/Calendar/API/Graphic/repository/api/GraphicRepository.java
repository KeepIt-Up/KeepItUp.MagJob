package com.keepitup.calendar.api.Calendar.API.Graphic.repository.api;

import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface GraphicRepository extends JpaRepository<Graphic, BigInteger> {
    Optional<Graphic> findById(UUID uuid);
    Page<Graphic> findAllByManagerId(UUID managerId, Pageable page);
    Page<Graphic> findGraphicsByManagerId(UUID managerId, Pageable page);
}
