package com.keepitup.calendar.api.Calendar.API.Graphic.service.impl;

import com.keepitup.calendar.api.Calendar.API.Graphic.service.api.GraphicService;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import com.keepitup.calendar.api.Calendar.API.Graphic.repository.api.GraphicRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class GraphicDefaultService implements GraphicService {
    private final GraphicRepository graphicRepository;

    @Autowired
    public GraphicDefaultService(GraphicRepository graphicRepository) {
        this.graphicRepository = graphicRepository;
    }


    @Override
    public Optional<Page<Graphic>> findAllGraphicsByUser(UUID userId, PageRequest pageRequest) {
        return Optional.empty();
    }

    @Override
    public List<Graphic> findAll() {
        return graphicRepository.findAll();
    }

    @Override
    public Page<Graphic> findAll(Pageable pageable) {
        return graphicRepository.findAll(pageable);
    }

    @Override
    public Optional<Graphic> find(UUID id) {
        return graphicRepository.findById(id);
    }

    @Override
    public void create(Graphic graphic) {
        graphicRepository.save(graphic);
    }

    @Override
    public void delete(UUID id) {
        graphicRepository.findById(id).ifPresent(graphicRepository::delete);
    }

    @Override
    public void update(Graphic graphic) {
        graphicRepository.save(graphic);
    }
}
