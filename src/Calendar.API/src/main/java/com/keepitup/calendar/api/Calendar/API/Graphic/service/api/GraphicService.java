package com.keepitup.calendar.api.Calendar.API.Graphic.service.api;

import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface GraphicService {
    Optional<Page<Graphic>> findAllGraphicsByUser(UUID userId, PageRequest pageRequest);

    List<Graphic> findAll();

    Page<Graphic> findAll(Pageable pageable);

    Optional<Graphic> find(UUID id);

    void create(Graphic graphic);

    void delete(UUID id);

    void update(Graphic graphic);
}
