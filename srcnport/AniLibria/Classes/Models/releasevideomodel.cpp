#include "releasevideomodel.h"

ReleaseVideoModel::ReleaseVideoModel(QObject *parent) : QObject(parent)
{

}

int ReleaseVideoModel::id() const
{
    return m_Id;
}

void ReleaseVideoModel::setId(const int id)
{
    if (id == m_Id) return;

    m_Id = id;
    emit idChanged();
}

QString ReleaseVideoModel::title() const
{
    return m_Title;
}

void ReleaseVideoModel::setTitle(const QString &title)
{
    if (title == m_Title) return;

    m_Title = title;
    emit titleChanged();
}

QString ReleaseVideoModel::sd() const
{
    return m_Sd;
}

void ReleaseVideoModel::setSd(const QString &sd)
{
    if (sd == m_Sd) return;

    m_Sd = sd;
    emit sdChanged();
}

QString ReleaseVideoModel::hd() const
{
    return m_Hd;
}

void ReleaseVideoModel::setHd(const QString &hd)
{
    if (hd == m_Hd) return;

    m_Hd = hd;
    emit hdChanged();
}

QString ReleaseVideoModel::fullhd() const
{
    return m_FullHd;
}

void ReleaseVideoModel::setFullhd(const QString &fullhd)
{
    if (fullhd == m_FullHd) return;

    m_FullHd = fullhd;
    emit fullhdChanged();
}

QString ReleaseVideoModel::srcsd() const
{
    return m_SrcSd;
}

void ReleaseVideoModel::setSrcsd(const QString &srcsd)
{
    if (srcsd == m_SrcSd) return;

    m_SrcSd = srcsd;
    emit srcsdChanged();
}

QString ReleaseVideoModel::srchd() const
{
    return m_SrcHd;
}

void ReleaseVideoModel::setSrchd(const QString &srchd)
{
    if (srchd == m_SrcHd) return;

    m_SrcHd = srchd;
    emit srchdChanged();
}
