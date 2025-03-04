package com.keepitup.workevidence.api.WorkEvidence.API.Member.entity;

//import com.keepitup.magjobbackend.announcementreceiver.entity.AnnouncementReceiver;
//import com.keepitup.magjobbackend.assignee.entity.Assignee;
//import com.keepitup.magjobbackend.materialreceiver.entity.MaterialReceiver;
//import com.keepitup.magjobbackend.organization.entity.Organization;
//import com.keepitup.magjobbackend.rolemember.entity.RoleMember;
//import com.keepitup.magjobbackend.user.entity.User;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotNull;
import lombok.*;
import lombok.experimental.SuperBuilder;

import java.math.BigInteger;
import java.util.List;

@Getter
@Setter
@SuperBuilder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Entity
@Table(name = "members")
public class Member {
    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "memberSequenceGenerator")
    @SequenceGenerator(name = "memberSequenceGenerator")
    private BigInteger id;

    @NotNull
    @Column(name = "name", nullable = false)
    String pseudonym;

    @NotNull
    @Column(name = "is_still_member", nullable = false)
    Boolean isStillMember;

//    @ManyToOne
//    @JoinColumn(name = "organization_id")
//    private Organization organization;
//
//    @ManyToOne
//    @JoinColumn(name = "user_id")
//    private User user;
//
//    @OneToMany(mappedBy = "member")
//    private List<Assignee> assignees;
//
//    @OneToMany(mappedBy = "member")
//    private List<AnnouncementReceiver> announcementReceivers;
//
//    @OneToMany(mappedBy = "member")
//    private List<MaterialReceiver> materialReceivers;
//
//    @OneToMany(mappedBy = "member")
//    private List<RoleMember> roleMembers;
}