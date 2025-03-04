package com.keepitup.workevidence.api.WorkEvidence.API.shift.repository.api;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface ShiftRepository extends JpaRepository<Shift, BigInteger> {
}
